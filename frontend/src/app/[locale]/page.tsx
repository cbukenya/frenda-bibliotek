'use client';

import { Suspense, useEffect, useMemo, useState } from 'react';
import { useSearchParams } from 'next/navigation';
import { useTranslations } from 'next-intl';
import { getBooks, type Book } from '@/lib/api';
import BookCard from '@/components/BookCard';

function BrowseContent() {
  const t = useTranslations('browse');
  const searchParams = useSearchParams();
  const [books, setBooks] = useState<Book[]>([]);
  const [loading, setLoading] = useState(true);
  const [query, setQuery] = useState(searchParams.get('q') ?? '');

  const load = () => {
    setLoading(true);
    getBooks().then(setBooks).finally(() => setLoading(false));
  };

  useEffect(() => { load(); }, []);

  const filtered = useMemo(() => {
    const q = query.toLowerCase().trim();
    if (!q) return books;
    return books.filter(b =>
      b.title.toLowerCase().includes(q) ||
      b.author.toLowerCase().includes(q) ||
      b.genre.toLowerCase().includes(q) ||
      b.isbn.includes(q)
    );
  }, [books, query]);

  return (
    <div className="container">
      <div className="page-header">
        <h1 className="page-title">{t('title')}</h1>
        <p className="page-subtitle">{t('subtitle')}</p>
      </div>

      <div className="search-bar">
        <svg className="search-bar__icon" width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
          <circle cx="11" cy="11" r="8"/><path d="m21 21-4.35-4.35"/>
        </svg>
        <input
          type="search"
          placeholder={t('searchPlaceholder')}
          value={query}
          onChange={e => setQuery(e.target.value)}
        />
      </div>

      {loading ? (
        <div className="book-grid">
          {Array.from({ length: 6 }).map((_, i) => (
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
      ) : filtered.length === 0 ? (
        <div className="empty-state">
          <div className="empty-state__icon">📭</div>
          <div className="empty-state__title">{t('noResults')}</div>
          <p>{t('noResultsHint')}</p>
        </div>
      ) : (
        <div className="book-grid">
          {filtered.map(book => (
            <BookCard key={book.id} book={book} onBorrowed={load} />
          ))}
        </div>
      )}
    </div>
  );
}

export default function BrowsePage() {
  return (
    <Suspense>
      <BrowseContent />
    </Suspense>
  );
}
