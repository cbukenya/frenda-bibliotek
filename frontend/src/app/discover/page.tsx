'use client';

import { useEffect, useState } from 'react';
import { getTopBooks, getMyLoans, getBook, type Book, type Loan } from '@/lib/api';
import TopListRow from '@/components/TopListRow';
import BookCard from '@/components/BookCard';

export default function DiscoverPage() {
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

        // Use the most recently borrowed book's recommendations
        const recent = loans.filter(l => !l.returnedAt).sort(
          (a, b) => new Date(b.borrowedAt).getTime() - new Date(a.borrowedAt).getTime()
        )[0];

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
    <div className="container">
      <div className="page-header">
        <h1 className="page-title">Discover New Worlds</h1>
        <p className="page-subtitle">
          Explore curated recommendations and top trending titles handpicked for your reading journey.
        </p>
      </div>

      {/* Recommendations section */}
      {(loading || recommendations.length > 0) && (
        <>
          <div className="section-header">
            <h2 className="section-title">
              Recommended for You
            </h2>
            {basisTitle && (
              <span className="section-link" style={{ color: 'var(--gray-400)', fontSize: '.8rem' }}>
                Because you borrowed "{basisTitle}"
              </span>
            )}
          </div>

          {loading ? (
            <div className="book-grid" style={{ marginBottom: '2.5rem' }}>
              {Array.from({ length: 4 }).map((_, i) => (
                <div key={i} style={{ borderRadius: 12, overflow: 'hidden', background: '#fff', boxShadow: 'var(--shadow-sm)' }}>
                  <div className="skeleton" style={{ aspectRatio: '3/4' }} />
                  <div style={{ padding: '1rem', display: 'flex', flexDirection: 'column', gap: '.5rem' }}>
                    <div className="skeleton" style={{ height: 16, borderRadius: 4 }} />
                    <div className="skeleton" style={{ height: 12, width: '60%', borderRadius: 4 }} />
                    <div className="skeleton" style={{ height: 34, borderRadius: 6, marginTop: '.5rem' }} />
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <div className="book-grid" style={{ marginBottom: '2.5rem' }}>
              {recommendations.map(book => (
                <BookCard key={book.id} book={book} />
              ))}
            </div>
          )}
        </>
      )}

      {/* Top list */}
      <div className="section-header">
        <h2 className="section-title">Top List: Most Borrowed This Month</h2>
      </div>

      {loading ? (
        <div className="top-list">
          {Array.from({ length: 5 }).map((_, i) => (
            <div key={i} className="skeleton" style={{ height: 76, borderRadius: 12 }} />
          ))}
        </div>
      ) : (
        <div className="top-list">
          {topBooks.map((book, i) => (
            <TopListRow key={book.id} book={book} rank={i + 1} />
          ))}
        </div>
      )}
    </div>
  );
}
