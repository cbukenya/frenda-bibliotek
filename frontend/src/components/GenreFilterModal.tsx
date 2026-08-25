'use client';

import { useEffect, useState } from 'react';
import { useTranslations } from 'next-intl';
import { getGenreTree, type Genre } from '@/lib/api';

interface Props {
  open: boolean;
  onSelect: (genre: Genre | null) => void;
}

export default function GenreFilterPanel({ open, onSelect }: Props) {
  const t = useTranslations('browse');
  const [tree, setTree] = useState<Genre[]>([]);
  const [path, setPath] = useState<Genre[]>([]);

  useEffect(() => {
    if (open && tree.length === 0) {
      getGenreTree().then(setTree).catch(() => []);
    }
  }, [open, tree.length]);

  if (!open) return null;

  const current = path.length === 0
    ? tree
    : path[path.length - 1].children ?? [];

  const drillDown = (genre: Genre) => {
    if (genre.children && genre.children.length > 0) {
      setPath(prev => [...prev, genre]);
    } else {
      onSelect(genre);
      setPath([]);
    }
  };

  const goToLevel = (index: number) => {
    if (index < 0) {
      setPath([]);
    } else {
      setPath(prev => prev.slice(0, index + 1));
    }
  };

  return (
    <div className="w-full border border-outline-variant/20 rounded-xl bg-surface-container-lowest mt-3 overflow-hidden">
      {/* Breadcrumb nav inside panel */}
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

      {/* Genre grid */}
      <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-1 p-3">
        {current.map(genre => (
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

      {/* Select current level button */}
      {path.length > 0 && (
        <div className="px-5 pb-3 flex gap-2">
          <button
            onClick={() => { onSelect(path[path.length - 1]); setPath([]); }}
            className="px-4 py-1.5 rounded-lg bg-primary text-on-primary text-sm font-label-md hover:opacity-90 transition-opacity"
          >
            {t('applyFilter')}: {path[path.length - 1].name}
          </button>
          <button
            onClick={() => { onSelect(null); setPath([]); }}
            className="px-4 py-1.5 rounded-lg border border-outline-variant/30 text-sm font-label-md text-on-surface-variant hover:bg-surface-container-low transition-colors"
          >
            {t('clearFilter')}
          </button>
        </div>
      )}
    </div>
  );
}
