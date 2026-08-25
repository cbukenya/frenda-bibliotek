'use client';

import { useEffect, useState } from 'react';
import Link from 'next/link';
import { useParams, useRouter } from 'next/navigation';
import { getBook, borrowBook, type BookDetail, genreEmoji, estReadingHours } from '@/lib/api';

export default function BookDetailPage() {
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
    <div className="container">
      <div className="book-detail">
        <div className="skeleton" style={{ height: 20, width: 120, borderRadius: 4, marginBottom: '1.5rem' }} />
        <div className="book-detail__hero">
          <div className="skeleton" style={{ aspectRatio: '3/4', borderRadius: 12 }} />
          <div style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
            <div className="skeleton" style={{ height: 40, borderRadius: 4 }} />
            <div className="skeleton" style={{ height: 20, width: '40%', borderRadius: 4 }} />
            <div className="skeleton" style={{ height: 80, borderRadius: 4 }} />
          </div>
        </div>
      </div>
    </div>
  );

  if (!book) return null;

  const available = book.availableCopies > 0;

  return (
    <div className="container">
      <div className="book-detail">
        <Link href="/" className="book-detail__back">
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><polyline points="15 18 9 12 15 6"/></svg>
          Back to results
        </Link>

        <div className="book-detail__hero">
          {/* Cover */}
          <div className="book-detail__cover">
            <span className={`book-card__badge ${available ? 'badge--available' : 'badge--unavailable'}`} style={{ position: 'absolute', top: '.6rem', right: '.6rem' }}>
              {available ? `${book.availableCopies} of ${book.totalCopies}` : 'All Out'}
            </span>
            {book.coverUrl ? (
              <img src={book.coverUrl} alt={book.title} />
            ) : (
              <div style={{ width: '100%', height: '100%', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: '5rem', background: 'var(--gray-100)' }}>
                {genreEmoji(book.genre)}
              </div>
            )}
          </div>

          {/* Info */}
          <div>
            <span className="book-card__badge badge--genre" style={{ display: 'inline-block', marginBottom: '.75rem', position: 'static' }}>
              {book.genre}
            </span>

            <h1 className="book-detail__title">{book.title}</h1>
            <p className="book-detail__author">{book.author} · {book.publishedYear}</p>
            <p className="book-detail__description">{book.description}</p>

            <div className="book-detail__actions">
              <button
                className="btn btn--primary"
                onClick={handleBorrow}
                disabled={!available || borrowing || borrowed}
              >
                <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
                  <path d="M4 19.5A2.5 2.5 0 0 1 6.5 17H20"/><path d="M6.5 2H20v20H6.5A2.5 2.5 0 0 1 4 19.5v-15A2.5 2.5 0 0 1 6.5 2z"/>
                </svg>
                {borrowing ? 'Borrowing…' : borrowed ? 'Borrowed ✓' : 'Borrow Book'}
              </button>
              <Link href="/loans" className="btn btn--outline">
                <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                  <path d="M20.84 4.61a5.5 5.5 0 0 0-7.78 0L12 5.67l-1.06-1.06a5.5 5.5 0 0 0-7.78 7.78l1.06 1.06L12 21.23l7.78-7.78 1.06-1.06a5.5 5.5 0 0 0 0-7.78z"/>
                </svg>
                My Loans
              </Link>
            </div>

            {/* Metadata row */}
            <div className="book-detail__meta">
              <div>
                <div className="meta-item__label">ISBN</div>
                <div className="meta-item__value">{book.isbn}</div>
              </div>
              <div>
                <div className="meta-item__label">Genre</div>
                <div className="meta-item__value">{book.genre}</div>
              </div>
              <div>
                <div className="meta-item__label">Pages</div>
                <div className="meta-item__value">{book.totalPages}</div>
              </div>
              <div>
                <div className="meta-item__label">Year</div>
                <div className="meta-item__value">{book.publishedYear}</div>
              </div>
            </div>

            {book.avgReadingDays && (
              <div className="reading-time-banner">
                <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                  <circle cx="12" cy="12" r="10"/><polyline points="12 6 12 12 16 14"/>
                </svg>
                Avg loan: {Math.round(book.avgReadingDays)} days · Est. {estReadingHours(book.totalPages)}
              </div>
            )}
          </div>
        </div>

        {/* Recommendations */}
        {book.recommendations.length > 0 && (
          <div className="recommendations">
            <div className="section-header">
              <h2 className="section-title">Others who borrowed this also borrowed…</h2>
            </div>
            <div className="rec-grid">
              {book.recommendations.map(rec => (
                <Link key={rec.id} href={`/books/${rec.id}`} className="rec-card">
                  <div className="rec-card__cover">
                    {rec.coverUrl ? (
                      <img src={rec.coverUrl} alt={rec.title} />
                    ) : (
                      <div style={{ width: '100%', height: '100%', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: '2.5rem', background: 'var(--gray-100)' }}>
                        {genreEmoji(rec.genre)}
                      </div>
                    )}
                  </div>
                  <div className="rec-card__title">{rec.title}</div>
                  <div className="rec-card__author">{rec.author}</div>
                </Link>
              ))}
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
