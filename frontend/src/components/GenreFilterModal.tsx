'use client';

import { useEffect, useState } from 'react';
import { useTranslations } from 'next-intl';
import { getGenreTree, type Genre, genreEmoji } from '@/lib/api';

interface Props {
  open: boolean;
  onClose: () => void;
  onSelect: (genre: Genre | null, path: Genre[]) => void;
}

export default function GenreFilterModal({ open, onClose, onSelect }: Props) {
  const t = useTranslations('browse');
  const [tree, setTree] = useState<Genre[]>([]);
  const [path, setPath] = useState<Genre[]>([]);

  useEffect(() => {
    if (open && tree.length === 0) {
      getGenreTree().then(setTree).catch(() => []);
    }
  }, [open, tree.length]);

  if (!open) return null;

  // Current level: root or children of the last genre in the path
  const current = path.length === 0
    ? tree
    : path[path.length - 1].children ?? [];

  const drillDown = (genre: Genre) => {
    if (genre.children && genre.children.length > 0) {
      setPath(prev => [...prev, genre]);
    } else {
      // Leaf genre — select it
      onSelect(genre, [...path, genre]);
    }
  };

  const goToLevel = (index: number) => {
    if (index < 0) {
      setPath([]);
    } else {
      setPath(prev => prev.slice(0, index + 1));
    }
  };

  const selectCurrent = () => {
    if (path.length === 0) {
      onSelect(null, []);
    } else {
      const genre = path[path.length - 1];
      onSelect(genre, [...path]);
    }
  };

  return (
    <div className="fixed inset-0 z-[100] flex items-center justify-center bg-black/40 backdrop-blur-sm" onClick={onClose}>
      <div
        className="bg-surface-container-lowest rounded-2xl shadow-2xl w-full max-w-md mx-4 overflow-hidden"
        onClick={e => e.stopPropagation()}
      >
        {/* Header */}
        <div className="flex items-center justify-between px-6 py-4 border-b border-outline-variant/20">
          <h2 className="font-headline-sm text-headline-sm text-on-surface">{t('filterByGenre')}</h2>
          <button onClick={onClose} className="p-1 text-on-surface-variant hover:text-primary transition-colors">
            <span className="material-symbols-outlined">close</span>
          </button>
        </div>

        {/* Breadcrumbs */}
        <div className="px-6 pt-4 pb-2 flex items-center gap-1 text-sm flex-wrap">
          <button
            onClick={() => goToLevel(-1)}
            className={`hover:text-primary transition-colors ${path.length === 0 ? 'text-primary font-bold' : 'text-on-surface-variant'}`}
          >
            {t('allBooks')}
          </button>
          {path.map((g, i) => (
            <span key={g.id} className="flex items-center gap-1">
              <span className="text-outline">/</span>
              <button
                onClick={() => goToLevel(i)}
                className={`hover:text-primary transition-colors ${i === path.length - 1 ? 'text-primary font-bold' : 'text-on-surface-variant'}`}
              >
                {g.name}
              </button>
            </span>
          ))}
        </div>

        {/* Genre list */}
        <div className="px-4 pb-2 max-h-80 overflow-y-auto">
          {current.map(genre => (
            <button
              key={genre.id}
              onClick={() => drillDown(genre)}
              className="w-full flex items-center justify-between px-4 py-3 rounded-lg hover:bg-surface-container-low transition-colors text-left"
            >
              <span className="flex items-center gap-3">
                <span className="text-lg">{genreEmoji(genre.name)}</span>
                <span className="font-body-md text-body-md text-on-surface">{genre.name}</span>
              </span>
              {genre.children && genre.children.length > 0 && (
                <span className="material-symbols-outlined text-on-surface-variant text-[18px]">chevron_right</span>
              )}
            </button>
          ))}
        </div>

        {/* Actions */}
        <div className="px-6 py-4 border-t border-outline-variant/20 flex gap-3">
          <button
            onClick={() => { onSelect(null, []); }}
            className="flex-1 py-2.5 rounded-lg border border-outline-variant/30 font-label-md text-label-md text-on-surface-variant hover:bg-surface-container-low transition-colors"
          >
            {t('clearFilter')}
          </button>
          <button
            onClick={selectCurrent}
            className="flex-1 py-2.5 rounded-lg bg-primary text-on-primary font-label-md text-label-md hover:opacity-90 transition-opacity"
          >
            {t('applyFilter')}
          </button>
        </div>
      </div>
    </div>
  );
}
