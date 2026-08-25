'use client';

import { useEffect, useState } from 'react';
import Link from 'next/link';
import { useTranslations } from 'next-intl';
import { getTopBooks, getMyLoans, getBook, type Book, estReadingHours, genreEmoji, borrowBook } from '@/lib/api';
import TopListRow from '@/components/TopListRow';

export default function DiscoverPage() {
  const t = useTranslations('discover');
  const [topBooks, setTopBooks] = useState<Book[]>([]);
  const [recommendations, setRecommendations] = useState<Book[]>([]);
  const [basisTitle, setBasisTitle] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const load = async () => {
      setLoading(true);
      try {
        const [top, loans] = await Promise.all([getTopBooks(), getMyLoans()]);
        setTopBooks(top);
        const recent = loans
          .filter(l => !l.returnedAt)
          .sort((a, b) => new Date(b.borrowedAt).getTime() - new Date(a.borrowedAt).getTime())[0];
        if (recent) {
          const detail = await getBook(recent.bookId);
          setRecommendations(detail.recommendations);
          setBasisTitle(detail.title);
        }
      } finally {
        setLoading(false);
      }
    };
    load();
    window.addEventListener('frenda_user_changed', load);
    return () => window.removeEventListener('frenda_user_changed', load);
  }, []);

  return (
    <div className="pt-24 pb-20 md:pb-8">
      <main className="max-w-container-max mx-auto px-margin-mobile md:px-margin-desktop py-8 flex flex-col gap-12">

        {/* Header */}
        <header>
          <h1 className="font-headline-xl text-headline-xl mb-2 text-text-main">{t('title')}</h1>
          <p className="font-body-lg text-body-lg text-on-surface-variant max-w-[600px]">{t('subtitle')}</p>
        </header>

        {/* Recommendations */}
        {(loading || recommendations.length > 0) && (
          <section>
            <div className="flex items-end justify-between mb-6">
              <div>
                <h2 className="font-headline-md text-headline-md text-text-main">{t('recommended')}</h2>
                {basisTitle && (
                  <p className="font-body-sm text-body-sm text-on-surface-variant mt-1">
                    {t('because', { title: basisTitle })}
                  </p>
                )}
              </div>
              <Link href="/" className="text-primary font-label-md text-label-md flex items-center gap-1">
                See more <span className="material-symbols-outlined text-[18px]">arrow_forward</span>
              </Link>
            </div>

            {loading ? (
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-gutter">
                {Array.from({ length: 4 }).map((_, i) => (
                  <div key={i} className="rounded-xl overflow-hidden bg-surface-container-lowest shadow-sm animate-pulse">
                    <div className="aspect-[2/3] bg-surface-container-high" />
                    <div className="p-6 space-y-3">
                      <div className="h-4 bg-surface-container-high rounded w-3/4" />
                      <div className="h-10 bg-surface-container-high rounded" />
                    </div>
                  </div>
                ))}
              </div>
            ) : (
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-gutter">
                {recommendations.map(book => (
                  <RecommendationCard key={book.id} book={book} />
                ))}
              </div>
            )}
          </section>
        )}

        {/* Top list */}
        <section>
          <h2 className="font-headline-md text-headline-md text-text-main mb-6">{t('topList')}</h2>
          {loading ? (
            <div className="flex flex-col gap-4">
              {Array.from({ length: 5 }).map((_, i) => (
                <div key={i} className="rounded-xl bg-surface-container-lowest shadow-sm h-20 animate-pulse" />
              ))}
            </div>
          ) : (
            <div className="flex flex-col gap-4">
              {topBooks.map((book, i) => (
                <TopListRow key={book.id} book={book} rank={i + 1} />
              ))}
            </div>
          )}
        </section>
      </main>
    </div>
  );
}

// Inline recommendation card (matches Stitch discover card exactly)
function RecommendationCard({ book }: { book: Book }) {
  const t = useTranslations('bookCard');
  const [loading, setLoading] = useState(false);
  const [borrowed, setBorrowed] = useState(false);

  const handleBorrow = async (e: React.MouseEvent) => {
    e.preventDefault();
    if (loading || borrowed) return;
    setLoading(true);
    try { await borrowBook(book.isbn); setBorrowed(true); }
    catch (err) { alert((err as Error).message); }
    finally { setLoading(false); }
  };

  return (
    <Link href={`/books/${book.id}`} style={{ textDecoration: 'none' }}>
      <article className="bg-surface-container-lowest rounded-xl shadow-sm border border-transparent hover:border-secondary-container transition-all overflow-hidden flex flex-col group h-full">
        <div className="aspect-[2/3] bg-surface-container-low overflow-hidden">
          {book.coverUrl ? (
            <img src={book.coverUrl} alt={book.title} className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300" />
          ) : (
            <div className="w-full h-full flex items-center justify-center text-5xl bg-surface-container">
              {genreEmoji(book.genre)}
            </div>
          )}
        </div>
        <div className="p-6 flex flex-col flex-1">
          <div className="flex items-center gap-2 mb-3 text-on-surface-variant font-label-sm text-label-sm">
            <span className="material-symbols-outlined text-[16px]">schedule</span>
            {estReadingHours(book.totalPages)} read
          </div>
          <h3 className="font-headline-sm text-headline-sm mb-1 line-clamp-2 group-hover:text-primary transition-colors">
            {book.title}
          </h3>
          <p className="font-body-sm text-body-sm text-on-surface-variant mb-6">{book.author}</p>
          <button
            onClick={handleBorrow}
            disabled={loading || borrowed || book.availableCopies === 0}
            className="mt-auto w-full bg-primary text-on-primary py-2.5 rounded-lg font-label-md text-label-md hover:opacity-90 transition-opacity disabled:opacity-60"
          >
            {loading ? t('borrowing') : borrowed ? t('borrowed') : t('borrow')}
          </button>
        </div>
      </article>
    </Link>
  );
}
