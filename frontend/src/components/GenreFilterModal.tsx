'use client';

import { useEffect, useState } from 'react';
import { useTranslations } from 'next-intl';
import { getGenreTree, getAuthors, type Genre, type Author } from '@/lib/api';

type FilterTab = 'genre' | 'author';

export interface FilterSelection {
  genre: Genre | null;
  author: Author | null;
}

interface Props {
  open: boolean;
  activeFilters: FilterSelection;
  onSelect: (filters: FilterSelection) => void;
}

export default function GenreFilterPanel({ open, activeFilters, onSelect }: Props) {
  const t = useTranslations('browse');
  const [tab, setTab] = useState<FilterTab>('genre');
  const [tree, setTree] = useState<Genre[]>([]);
  const [authors, setAuthors] = useState<Author[]>([]);
  const [authorSearch, setAuthorSearch] = useState('');
  const [path, setPath] = useState<Genre[]>([]);

  useEffect(() => {
    if (open && tree.length === 0) {
      getGenreTree().then(setTree).catch(() => []);
    }
    if (open && authors.length === 0) {
      getAuthors().then(setAuthors).catch(() => []);
    }
  }, [open, tree.length, authors.length]);

  if (!open) return null;

  // Genre drill-down
  const currentGenres = path.length === 0
    ? tree
    : path[path.length - 1].children ?? [];

  const drillDown = (genre: Genre) => {
    if (genre.children && genre.children.length > 0) {
      setPath(prev => [...prev, genre]);
    } else {
      onSelect({ ...activeFilters, genre });
      setPath([]);
    }
  };

  const goToLevel = (index: number) => {
    if (index < 0) setPath([]);
    else setPath(prev => prev.slice(0, index + 1));
  };

  // Author filtering
  const filteredAuthors = authorSearch.trim()
    ? authors.filter(a => a.name.toLowerCase().includes(authorSearch.toLowerCase()))
    : authors;

  return (
    <div className="w-full border border-outline-variant/20 rounded-xl bg-surface-container-lowest mt-3 overflow-hidden">
      {/* Tabs */}
      <div className="flex border-b border-outline-variant/20">
        <button
          onClick={() => setTab('genre')}
          className={`flex-1 py-2.5 text-sm font-label-md transition-colors ${
            tab === 'genre'
              ? 'text-primary border-b-2 border-primary'
              : 'text-on-surface-variant hover:text-on-surface'
          }`}
        >
          {t('filterByGenre')}
        </button>
        <button
          onClick={() => setTab('author')}
          className={`flex-1 py-2.5 text-sm font-label-md transition-colors ${
            tab === 'author'
              ? 'text-primary border-b-2 border-primary'
              : 'text-on-surface-variant hover:text-on-surface'
          }`}
        >
          {t('filterByAuthor')}
        </button>
      </div>

      {/* ── Genre tab ── */}
      {tab === 'genre' && (
        <>
          {path.length > 0 && (
            <div className="px-5 pt-3 pb-1 flex items-center gap-1.5 text-xs flex-wrap border-b border-outline-variant/10">
              <button
                onClick={() => goToLevel(-1)}
                className="text-on-surface-variant hover:text-primary transition-colors"
              >
                {t('allBooks')}
              </button>
              {path.map((g, i) => (
                <span key={g.id} className="flex items-center gap-1.5">
                  <span className="text-outline">/</span>
                  <button
                    onClick={() => goToLevel(i)}
                    className={`hover:text-primary transition-colors ${i === path.length - 1 ? 'text-primary font-semibold' : 'text-on-surface-variant'}`}
                  >
                    {g.name}
                  </button>
                </span>
              ))}
            </div>
          )}

          <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-1 p-3">
            {currentGenres.map(genre => (
              <button
                key={genre.id}
                onClick={() => drillDown(genre)}
                className="flex items-center justify-between px-3 py-2.5 rounded-lg hover:bg-surface-container-low transition-colors text-left"
              >
                <span className="font-body-md text-sm text-on-surface truncate">{genre.name}</span>
                {genre.children && genre.children.length > 0 && (
                  <span className="material-symbols-outlined text-on-surface-variant text-[14px] ml-auto flex-shrink-0">chevron_right</span>
                )}
              </button>
            ))}
          </div>

          {path.length > 0 && (
            <div className="px-5 pb-3 flex gap-2">
              <button
                onClick={() => { onSelect({ ...activeFilters, genre: path[path.length - 1] }); setPath([]); }}
                className="px-4 py-1.5 rounded-lg bg-primary text-on-primary text-sm font-label-md hover:opacity-90 transition-opacity"
              >
                {t('applyFilter')}: {path[path.length - 1].name}
              </button>
              <button
                onClick={() => { onSelect({ ...activeFilters, genre: null }); setPath([]); }}
                className="px-4 py-1.5 rounded-lg border border-outline-variant/30 text-sm font-label-md text-on-surface-variant hover:bg-surface-container-low transition-colors"
              >
                {t('clearFilter')}
              </button>
            </div>
          )}
        </>
      )}

      {/* ── Author tab ── */}
      {tab === 'author' && (
        <div className="p-3">
          {/* Search */}
          <div className="relative mb-3">
            <span className="material-symbols-outlined absolute left-3 top-1/2 -translate-y-1/2 text-on-surface-variant text-[18px]">search</span>
            <input
              type="text"
              value={authorSearch}
              onChange={e => setAuthorSearch(e.target.value)}
              placeholder={t('searchAuthor')}
              className="w-full pl-9 pr-4 py-2 rounded-lg bg-surface-container-low text-sm text-on-surface placeholder:text-on-surface-variant/50 outline-none focus:ring-1 focus:ring-primary/30"
            />
          </div>

          {/* Author list */}
          <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 gap-1 max-h-60 overflow-y-auto">
            {filteredAuthors.map(author => (
              <button
                key={author.id}
                onClick={() => onSelect({ ...activeFilters, author })}
                className={`flex items-center px-3 py-2.5 rounded-lg transition-colors text-left ${
                  activeFilters.author?.id === author.id
                    ? 'bg-primary/10 text-primary font-semibold'
                    : 'hover:bg-surface-container-low text-on-surface'
                }`}
              >
                <span className="text-sm truncate">{author.name}</span>
              </button>
            ))}
          </div>

          {activeFilters.author && (
            <div className="pt-3">
              <button
                onClick={() => onSelect({ ...activeFilters, author: null })}
                className="px-4 py-1.5 rounded-lg border border-outline-variant/30 text-sm font-label-md text-on-surface-variant hover:bg-surface-container-low transition-colors"
              >
                {t('clearFilter')}
              </button>
            </div>
          )}
        </div>
      )}
    </div>
  );
}
