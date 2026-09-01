'use client';

interface Props {
  currentPage: number;
  totalPages: number;
  onPageChange: (page: number) => void;
}

export default function Pagination({ currentPage, totalPages, onPageChange }: Props) {
  if (totalPages <= 1) return null;

  // Build the visible page numbers: always show first, last, and a window around current
  const pages: (number | '...')[] = [];
  const addPage = (p: number) => {
    if (p >= 1 && p <= totalPages && !pages.includes(p)) pages.push(p);
  };

  addPage(1);
  if (currentPage > 3) pages.push('...');
  for (let i = Math.max(2, currentPage - 1); i <= Math.min(totalPages - 1, currentPage + 1); i++) {
    addPage(i);
  }
  if (currentPage < totalPages - 2) pages.push('...');
  addPage(totalPages);

  return (
    <nav aria-label="Pagination" className="flex items-center justify-center gap-2 mt-10">
      {/* Previous */}
      <button
        onClick={() => onPageChange(currentPage - 1)}
        disabled={currentPage === 1}
        className="flex items-center gap-1.5 px-4 py-2.5 rounded-lg font-label-md text-label-md
          bg-surface-container-low text-on-surface border border-outline-variant/30
          hover:bg-surface-container-high hover:border-primary/30 transition-all duration-200
          disabled:opacity-40 disabled:cursor-not-allowed disabled:hover:bg-surface-container-low disabled:hover:border-outline-variant/30"
      >
        <span className="material-symbols-outlined text-[18px]">chevron_left</span>
        Previous
      </button>

      {/* Page numbers */}
      {pages.map((p, i) =>
        p === '...' ? (
          <span key={`ellipsis-${i}`} className="w-10 text-center text-on-surface-variant font-body-md select-none">
            …
          </span>
        ) : (
          <button
            key={p}
            onClick={() => onPageChange(p)}
            className={`w-10 h-10 rounded-lg font-label-md text-label-md transition-all duration-200
              ${p === currentPage
                ? 'bg-primary text-on-primary shadow-md'
                : 'bg-surface-container-low text-on-surface border border-outline-variant/30 hover:bg-surface-container-high hover:border-primary/30'
              }`}
          >
            {p}
          </button>
        )
      )}

      {/* Next */}
      <button
        onClick={() => onPageChange(currentPage + 1)}
        disabled={currentPage === totalPages}
        className="flex items-center gap-1.5 px-4 py-2.5 rounded-lg font-label-md text-label-md
          bg-surface-container-low text-on-surface border border-outline-variant/30
          hover:bg-surface-container-high hover:border-primary/30 transition-all duration-200
          disabled:opacity-40 disabled:cursor-not-allowed disabled:hover:bg-surface-container-low disabled:hover:border-outline-variant/30"
      >
        Next
        <span className="material-symbols-outlined text-[18px]">chevron_right</span>
      </button>
    </nav>
  );
}
