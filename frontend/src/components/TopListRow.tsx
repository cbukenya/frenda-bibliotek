import Link from 'next/link';
import { useTranslations } from 'next-intl';
import { type Book, genreEmoji } from '@/lib/api';

interface Props {
  book: Book;
  rank: number;
}

const rankClass = (rank: number) => {
  if (rank === 1) return 'top-list-rank top-list-rank--gold';
  if (rank === 2) return 'top-list-rank top-list-rank--silver';
  if (rank === 3) return 'top-list-rank top-list-rank--bronze';
  return 'top-list-rank';
};

export default function TopListRow({ book, rank }: Props) {
  const t = useTranslations('topList');
  const totalLoans = book.totalCopies - book.availableCopies;

  return (
    <Link href={`/books/${book.id}`} className="top-list-row" style={{ textDecoration: 'none' }}>
      <span className={rankClass(rank)}>{rank}</span>

      <div className="top-list-thumb" style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', background: '#f1f3f5', borderRadius: '3px' }}>
        {book.coverUrl ? (
          <img src={book.coverUrl} alt={book.title} style={{ width: '40px', height: '52px', objectFit: 'cover', borderRadius: '3px' }} />
        ) : (
          <span style={{ fontSize: '1.4rem' }}>{genreEmoji(book.genre)}</span>
        )}
      </div>

      <div className="top-list-info">
        <div className="top-list-title">{book.title}</div>
        <div className="top-list-author">{book.author}</div>
        <div className="top-list-stat">
          <span className="stat-pill stat-pill--borrowed">
            {t('borrowed', { count: totalLoans })}
          </span>
          {book.availableCopies > 0 && (
            <span className="stat-pill stat-pill--available">{t('available')}</span>
          )}
        </div>
      </div>

      <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" style={{ color: '#adb5bd', flexShrink: 0 }}>
        <polyline points="9 18 15 12 9 6"/>
      </svg>
    </Link>
  );
}
