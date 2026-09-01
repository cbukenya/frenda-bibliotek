'use client';

import { useEffect, useMemo, useState, useCallback } from 'react';
import { useSearchParams } from 'next/navigation';
import { useTranslations } from 'next-intl';
import { getBooksPaged, getGenreAncestors, type Book, type Genre } from '@/lib/api';
import BookCard from '@/components/BookCard';
import Pagination from '@/components/Pagination';
import GenreFilterPanel, { type FilterSelection } from '@/components/GenreFilterModal';

const PAGE_SIZE = 9; // 3 columns × 3 rows on desktop

export default function BrowsePage() {
  const t = useTranslations('browse');
  const searchParams = useSearchParams();
  const [books, setBooks] = useState<Book[]>([]);
  const [totalBooks, setTotalBooks] = useState(0);
  const [currentPage, setCurrentPage] = useState(1);
  const [loading, setLoading] = useState(true);
  const [query, setQuery] = useState(searchParams.get('q') ?? '');

  // Filter state
  const [filterOpen, setFilterOpen] = useState(false);
  const [filters, setFilters] = useState<FilterSelection>({ genre: null, author: null });
  const [breadcrumb, setBreadcrumb] = useState<Genre[]>([]);

  const load = useCallback((page: number) => {
    setLoading(true);
    const params: { genreId?: number; authorId?: number } = {};
    if (filters.genre) params.genreId = filters.genre.id;
    if (filters.author) params.authorId = filters.author.id;
    getBooksPaged(page, PAGE_SIZE, Object.keys(params).length > 0 ? params : undefined)
      .then(result => {
        setBooks(result.items);
        setTotalBooks(result.total);
        setCurrentPage(result.page);
      })
      .finally(() => setLoading(false));
  }, [filters]);

  useEffect(() => { load(1); }, [load]);

  // Load breadcrumb ancestors when genre changes
  useEffect(() => {
    if (filters.genre) {
      getGenreAncestors(filters.genre.id).then(setBreadcrumb).catch(() => []);
    } else {
      setBreadcrumb([]);
    }
  }, [filters.genre]);

  // Reset to page 1 when filters change
  useEffect(() => {
    setCurrentPage(1);
  }, [filters]);

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

  const totalPages = Math.ceil(totalBooks / PAGE_SIZE);

  const handlePageChange = (page: number) => {
    setQuery(''); // Clear client-side filter when paginating
    load(page);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  const handleFilterSelect = (newFilters: FilterSelection) => {
    setFilters(newFilters);
    setFilterOpen(false);
  };

  const hasActiveFilter = filters.genre !== null || filters.author !== null;

  return (
    <div className="pt-24 pb-24 md:pb-8">
      <main className="max-w-container-max mx-auto px-margin-mobile md:px-margin-desktop pt-8">
        {/* Page header */}
        <header className="mb-8">
          <h1 className="font-headline-xl text-headline-xl text-primary mb-2 hidden md:block">{t('title')}</h1>
          <h1 className="font-headline-lg-mobile text-headline-lg-mobile text-primary mb-2 md:hidden">{t('title')}</h1>
          <p className="font-body-lg text-body-lg text-on-surface-variant max-w-2xl">{t('subtitle')}</p>
        </header>

        {/* Subtitle row: "Alla böcker" + breadcrumbs + filter */}
        <div className="flex items-center justify-between mb-2">
          <div className="flex items-center gap-1.5 flex-wrap">
            {/* "Alla böcker" — clickable subtitle that resets filter */}
            <button
              onClick={() => { setFilters({ genre: null, author: null }); setFilterOpen(false); }}
              className="font-headline-sm text-headline-sm text-on-surface hover:text-primary transition-colors"
            >
              {t('allBooks')}
            </button>

            {/* Genre breadcrumbs */}
            {breadcrumb.map((g, i) => (
              <span key={g.id} className="flex items-center gap-1.5">
                <span className="text-outline text-sm">/</span>
                <button
                  onClick={() => setFilters(f => ({ ...f, genre: g }))}
                  className={`text-sm hover:text-primary transition-colors ${
                    i === breadcrumb.length - 1
                      ? 'text-primary font-semibold'
                      : 'text-on-surface-variant'
                  }`}
                >
                  {g.name}
                </button>
              </span>
            ))}

            {/* Author chip */}
            {filters.author && (
              <span className="flex items-center gap-1.5">
                {breadcrumb.length > 0 && <span className="text-outline text-sm">·</span>}
                <span className="inline-flex items-center gap-1 px-2.5 py-0.5 rounded-full bg-primary/10 text-primary text-sm font-medium">
                  {filters.author.name}
                  <button
                    onClick={() => setFilters(f => ({ ...f, author: null }))}
                    className="hover:text-on-surface transition-colors"
                  >
                    <span className="material-symbols-outlined text-[14px]">close</span>
                  </button>
                </span>
              </span>
            )}
          </div>

          {/* Filter toggle */}
          <button
            onClick={() => setFilterOpen(v => !v)}
            className={`flex items-center gap-1.5 font-label-md text-label-md transition-colors ${
              filterOpen || hasActiveFilter ? 'text-primary' : 'text-on-surface-variant hover:text-primary'
            }`}
          >
            <span className="material-symbols-outlined text-[20px]">filter_list</span>
            {t('filter')}
          </button>
        </div>

        {/* Inline filter panel */}
        <GenreFilterPanel
          open={filterOpen}
          activeFilters={filters}
          onSelect={handleFilterSelect}
        />

        {/* Spacing when filter is closed */}
        {!filterOpen && <div className="mb-6" />}

        {/* Book grid */}
        {loading ? (
          <div className="bento-grid">
            {Array.from({ length: PAGE_SIZE }).map((_, i) => (
              <div key={i} className="rounded-xl overflow-hidden bg-surface-container-lowest shadow-sm animate-pulse">
                <div className="h-64 bg-surface-container-high" />
                <div className="p-gutter space-y-3">
                  <div className="h-4 bg-surface-container-high rounded w-3/4" />
                  <div className="h-3 bg-surface-container-high rounded w-1/2" />
                  <div className="h-10 bg-surface-container-high rounded mt-4" />
                </div>
              </div>
            ))}
          </div>
        ) : filtered.length === 0 ? (
          <div className="flex flex-col items-center justify-center py-24 text-on-surface-variant">
            <span className="material-symbols-outlined text-6xl mb-4 text-outline">inbox</span>
            <p className="font-headline-sm text-headline-sm mb-2">{t('noResults')}</p>
            <p className="font-body-md text-body-md">{t('noResultsHint')}</p>
          </div>
        ) : (
          <>
            <section className="bento-grid">
              {filtered.map(book => (
                <BookCard key={book.id} book={book} onBorrowed={() => load(currentPage)} />
              ))}
            </section>

            {/* Pagination */}
            <Pagination
              currentPage={currentPage}
              totalPages={totalPages}
              onPageChange={handlePageChange}
            />
          </>
        )}
      </main>
    </div>
  );
}
