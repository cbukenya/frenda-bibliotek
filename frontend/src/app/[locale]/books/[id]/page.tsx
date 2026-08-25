'use client';

import { useEffect, useState } from 'react';
import Link from 'next/link';
import { useParams, useRouter } from 'next/navigation';
import { useTranslations } from 'next-intl';
import { getBook, borrowBook, type BookDetail, genreEmoji, estReadingTime } from '@/lib/api';

export default function BookDetailPage() {
  const t = useTranslations('bookDetail');
  const { id } = useParams<{ id: string }>();
  const router = useRouter();
  const [book, setBook] = useState<BookDetail | null>(null);
  const [loading, setLoading] = useState(true);
  const [borrowing, setBorrowing] = useState(false);
  const [borrowed, setBorrowed] = useState(false);

  useEffect(() => {
    getBook(Number(id))
      .then(setBook)
      .catch(() => router.push('/'))
      .finally(() => setLoading(false));
  }, [id]);

  const handleBorrow = async () => {
    if (!book) return;
    setBorrowing(true);
    try {
      await borrowBook(book.isbn);
      setBorrowed(true);
      setBook(b => b ? { ...b, availableCopies: b.availableCopies - 1 } : b);
    } catch (err) {
      alert((err as Error).message);
    } finally {
      setBorrowing(false);
    }
  };

  if (loading) return (
    <div className="pt-24 pb-20 md:pb-8">
      <main className="max-w-container-max mx-auto px-margin-mobile md:px-margin-desktop py-8 animate-pulse">
        <div className="h-5 w-40 bg-surface-container-high rounded mb-8" />
        <div className="grid grid-cols-1 md:grid-cols-12 gap-gutter">
          <div className="md:col-span-4 lg:col-span-3">
            <div className="aspect-[2/3] rounded-xl bg-surface-container-high" />
          </div>
          <div className="md:col-span-8 space-y-4">
            <div className="h-10 bg-surface-container-high rounded w-3/4" />
            <div className="h-6 bg-surface-container-high rounded w-1/2" />
            <div className="h-24 bg-surface-container-high rounded" />
          </div>
        </div>
      </main>
    </div>
  );

  if (!book) return null;
  const available = book.availableCopies > 0;

  return (
    <div className="pt-24 pb-20 md:pb-8">
      <main className="max-w-container-max mx-auto px-margin-mobile md:px-margin-desktop py-8">
        {/* Back */}
        <button
          onClick={() => router.back()}
          className="inline-flex items-center gap-2 text-on-surface-variant hover:text-primary transition-colors font-body-sm text-body-sm mb-8 group"
        >
          <span className="material-symbols-outlined text-xl group-hover:-translate-x-1 transition-transform">arrow_back</span>
          {t('back')}
        </button>

        <div className="grid grid-cols-1 md:grid-cols-12 gap-gutter mb-16">
          {/* Cover column */}
          <div className="md:col-span-4 lg:col-span-3 flex flex-col gap-6">
            <div className="relative w-full aspect-[2/3] rounded-xl overflow-hidden shadow-[0_8px_30px_rgba(71,80,144,0.12)] bg-surface-container-low">
              {book.coverUrl ? (
                <img src={book.coverUrl} alt={book.title} className="w-full h-full object-cover" />
              ) : (
                <div className="w-full h-full flex items-center justify-center text-7xl bg-surface-container">
                  {genreEmoji(book.genre)}
                </div>
              )}
              {/* Availability */}
              <div className="absolute top-4 right-4 bg-surface-bright/90 backdrop-blur-sm px-3 py-1.5 rounded-full shadow-sm flex items-center gap-1.5 border border-status-available/20">
                <div className={`w-2 h-2 rounded-full ${available ? 'bg-status-available' : 'bg-status-borrowed'}`} />
                <span className={`font-label-sm text-label-sm font-semibold ${available ? 'text-status-available' : 'text-status-borrowed'}`}>
                  {available ? t('copies', { available: book.availableCopies, total: book.totalCopies }) : t('allOut')}
                </span>
              </div>
            </div>

            {/* Reading time card */}
            {book.avgReadingDays && (
              <div className="bg-surface-container-lowest border border-outline-variant/30 rounded-xl p-4 flex items-center gap-3 shadow-sm">
                <div className="w-10 h-10 rounded-full bg-primary-container/10 flex items-center justify-center text-primary">
                  <span className="material-symbols-outlined">schedule</span>
                </div>
                <div>
                  <p className="font-label-sm text-label-sm text-on-surface-variant uppercase tracking-wider">Avg loan</p>
                  <p className="font-headline-sm text-headline-sm text-on-background">{Math.round(book.avgReadingDays)} days</p>
                </div>
              </div>
            )}
          </div>

          {/* Info column */}
          <div className="md:col-span-8 lg:col-span-9 flex flex-col">
            <div className="mb-2">
              <span className="inline-block bg-secondary-container text-on-secondary-container font-label-sm text-label-sm px-3 py-1 rounded-full mb-4">
                {book.genre}
              </span>
            </div>

            <h1 className="font-headline-xl text-headline-xl text-on-background mb-2">{book.title}</h1>
            <h2 className="font-headline-md text-headline-md text-on-surface-variant font-normal mb-6">
              {book.author} · {book.publishedYear}
            </h2>
            <p className="font-body-lg text-body-lg text-on-background/80 mb-10 max-w-3xl leading-relaxed">
              {book.description}
            </p>

            {/* Actions */}
            <div className="flex flex-wrap items-center gap-4 pt-6 border-t border-outline-variant/30">
              <button
                onClick={handleBorrow}
                disabled={!available || borrowing || borrowed}
                className="bg-primary text-on-primary font-label-md text-label-md px-8 py-3.5 rounded-lg shadow-sm hover:opacity-90 transition-all flex items-center gap-2 disabled:opacity-60 disabled:cursor-not-allowed"
              >
                <span className="material-symbols-outlined">library_add</span>
                {borrowing ? t('borrowing') : borrowed ? t('borrowed') : t('borrowBook')}
              </button>
              <Link
                href="/loans"
                className="border-2 border-outline-variant text-on-surface-variant hover:bg-surface-container-low hover:text-primary hover:border-primary font-label-md text-label-md px-6 py-3 rounded-lg transition-all flex items-center gap-2"
              >
                <span className="material-symbols-outlined">favorite_border</span>
                {t('myLoans')}
              </Link>
            </div>

            {/* Metadata grid */}
            <div className="mt-12 grid grid-cols-2 md:grid-cols-4 gap-6 bg-surface-container-low p-6 rounded-xl">
              <div>
                <p className="font-label-sm text-label-sm text-on-surface-variant mb-1">{t('isbn')}</p>
                <p className="font-body-sm text-body-sm font-medium">{book.isbn}</p>
              </div>
              <div>
                <p className="font-label-sm text-label-sm text-on-surface-variant mb-1">{t('genre')}</p>
                <p className="font-body-sm text-body-sm font-medium">{book.genre}</p>
              </div>
              <div>
                <p className="font-label-sm text-label-sm text-on-surface-variant mb-1">{t('readingTime')}</p>
                <p className="font-body-sm text-body-sm font-medium">{estReadingTime(book.avgReadingDays) || '—'}</p>
              </div>
              <div>
                <p className="font-label-sm text-label-sm text-on-surface-variant mb-1">{t('year')}</p>
                <p className="font-body-sm text-body-sm font-medium">{book.publishedYear}</p>
              </div>
            </div>
          </div>
        </div>

        {/* Recommendations */}
        {book.recommendations.length > 0 && (
          <section>
            <h2 className="font-headline-md text-headline-md text-text-main mb-6">{t('recommendationsTitle')}</h2>
            <div className="grid grid-cols-2 md:grid-cols-4 gap-gutter">
              {book.recommendations.map(rec => (
                <Link key={rec.id} href={`/books/${rec.id}`} style={{ textDecoration: 'none' }}>
                  <div className="bg-surface-container-lowest rounded-xl overflow-hidden shadow-sm hover:shadow-[0_4px_20px_rgba(71,80,144,0.08)] transition-shadow border border-transparent hover:border-secondary-container">
                    <div className="aspect-[2/3] bg-surface-container flex items-center justify-center text-4xl">
                      {rec.coverUrl
                        ? <img src={rec.coverUrl} alt={rec.title} className="w-full h-full object-cover" />
                        : genreEmoji(rec.genre)
                      }
                    </div>
                    <div className="p-4">
                      <p className="font-headline-sm text-headline-sm text-text-main line-clamp-2 text-sm">{rec.title}</p>
                      <p className="font-body-sm text-body-sm text-on-surface-variant mt-1">{rec.author}</p>
                    </div>
                  </div>
                </Link>
              ))}
            </div>
          </section>
        )}
      </main>
    </div>
  );
}
