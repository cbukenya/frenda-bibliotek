'use client';

import Link from 'next/link';
import { useState } from 'react';
import { useTranslations } from 'next-intl';
import { type Book, borrowBook, estReadingHours, genreEmoji } from '@/lib/api';

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
    if (!available || loading) return;
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
    <Link href={`/books/${book.id}`} className="book-card" style={{ textDecoration: 'none' }}>
      <div className="book-card__cover">
        {book.coverUrl ? (
          <img src={book.coverUrl} alt={book.title} />
        ) : (
          <div className="book-card__cover-placeholder">{genreEmoji(book.genre)}</div>
        )}
        <span className={`book-card__badge ${available ? 'badge--available' : 'badge--unavailable'}`}>
          {available
            ? t('availableBadge', { available: book.availableCopies, total: book.totalCopies })
            : t('allOutBadge')}
        </span>
      </div>

      <div className="book-card__body">
        <div className="book-card__title">{book.title}</div>
        <div className="book-card__author">{book.author}</div>
        <div className="book-card__meta">
          <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <circle cx="12" cy="12" r="10"/><polyline points="12 6 12 12 16 14"/>
          </svg>
          {t('readingTime', { time: estReadingHours(book.totalPages) })}
          {' · '}
          {book.avgReadingDays
            ? t('avgLoan', { days: Math.round(book.avgReadingDays) })
            : t('communityData')}
        </div>
        <button
          className="btn btn--primary btn--sm"
          onClick={handleBorrow}
          disabled={!available || loading || borrowed}
        >
          {loading ? t('borrowing') : borrowed ? t('borrowed') : t('borrow')}
        </button>
      </div>
    </Link>
  );
}
