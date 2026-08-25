'use client';

import Link from 'next/link';
import { useState } from 'react';
import { useTranslations } from 'next-intl';
import { type Book, borrowBook, estReadingTime, genreEmoji } from '@/lib/api';

interface Props {
  book: Book;
  onBorrowed?: () => void;
}

export default function BookCard({ book, onBorrowed }: Props) {
  const t = useTranslations('bookCard');
  const [loading, setLoading] = useState(false);
  const [borrowed, setBorrowed] = useState(false);
  const available = book.availableCopies > 0;

  const handleBorrow = async (e: React.MouseEvent) => {
    e.preventDefault();
    e.stopPropagation();
    if (!available || loading || borrowed) return;
    setLoading(true);
    try {
      await borrowBook(book.isbn);
      setBorrowed(true);
      onBorrowed?.();
    } catch (err) {
      alert((err as Error).message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <Link href={`/books/${book.id}`} style={{ textDecoration: 'none' }}>
      <article className="cursor-pointer bg-surface-container-lowest rounded-xl shadow-[0_4px_20px_rgba(71,80,144,0.08)] hover:shadow-[0_8px_30px_rgba(71,80,144,0.12)] border border-transparent hover:border-secondary-container transition-all duration-300 flex flex-col overflow-hidden group h-full">
        {/* Cover */}
        <div className="relative h-64 bg-surface-container-low">
          {book.coverUrl ? (
            <img src={book.coverUrl} alt={book.title} className="w-full h-full object-cover" />
          ) : (
            <div className="w-full h-full flex items-center justify-center text-6xl bg-surface-container">
              {genreEmoji(book.genre)}
            </div>
          )}
          {/* Availability badge */}
          <div className="absolute top-4 right-4 px-3 py-1 bg-surface-container-lowest/80 backdrop-blur-sm rounded-full border border-outline/20 flex items-center gap-1.5">
            <span className={`w-2 h-2 rounded-full ${available ? 'bg-status-available' : 'bg-status-borrowed'}`} />
            <span className={`font-label-sm text-label-sm uppercase tracking-wider ${available ? 'text-status-available' : 'text-status-borrowed'}`}>
              {available
                ? t('availableBadge', { available: book.availableCopies, total: book.totalCopies })
                : t('allOutBadge')}
            </span>
          </div>
        </div>

        {/* Body */}
        <div className="p-gutter flex flex-col flex-grow">
          <h2 className="font-headline-sm text-headline-sm text-text-main mb-1 line-clamp-2">{book.title}</h2>
          <p className="font-body-sm text-body-sm text-on-surface-variant mb-4">{book.author}</p>

          <div className="mt-auto pt-4 border-t border-surface-container-highest">
            <div className="flex items-center gap-2 mb-4 text-on-surface-variant">
              <span className="material-symbols-outlined text-sm">schedule</span>
              <span className="font-label-sm text-label-sm">
              {estReadingTime(book.avgReadingDays) && t('readingTime', { time: estReadingTime(book.avgReadingDays) })}
              </span>
            </div>
            <button
              onClick={handleBorrow}
              disabled={!available || loading || borrowed}
              className={`w-full py-2.5 rounded-lg font-label-md text-label-md transition-colors ${
                available && !borrowed
                  ? 'bg-primary text-on-primary hover:opacity-90'
                  : 'bg-surface-container-highest text-on-surface-variant cursor-not-allowed opacity-70'
              }`}
            >
              {loading ? t('borrowing') : borrowed ? t('borrowed') : t('borrow')}
            </button>
          </div>
        </div>
      </article>
    </Link>
  );
}
