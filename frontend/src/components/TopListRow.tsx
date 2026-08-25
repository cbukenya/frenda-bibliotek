import Link from 'next/link';
import { useTranslations } from 'next-intl';
import { type Book, genreEmoji } from '@/lib/api';

interface Props { book: Book; rank: number; }

export default function TopListRow({ book, rank }: Props) {
  const t = useTranslations('topList');
  const totalLoans = book.totalCopies - book.availableCopies;

  const rankBg =
    rank === 1 ? 'bg-primary-container text-on-primary' :
    rank === 2 ? 'bg-surface-container-high text-on-surface' :
    rank === 3 ? 'bg-surface-container-high text-on-surface' :
                 'bg-surface-container text-on-surface-variant';

  return (
    <Link href={`/books/${book.id}`} style={{ textDecoration: 'none' }}>
      <div className="bg-surface-container-lowest rounded-xl p-6 shadow-sm flex items-center gap-6 hover:shadow-[0_4px_20px_rgba(71,80,144,0.08)] transition-shadow">
        <div className={`w-12 h-12 rounded-full flex items-center justify-center font-bold font-headline-sm text-headline-sm flex-shrink-0 ${rankBg}`}>
          {rank}
        </div>

        {/* Thumbnail */}
        <div className="w-10 h-14 flex-shrink-0 rounded overflow-hidden bg-surface-container flex items-center justify-center">
          {book.coverUrl
            ? <img src={book.coverUrl} alt={book.title} className="w-full h-full object-cover" />
            : <span className="text-xl">{genreEmoji(book.genre)}</span>
          }
        </div>

        <div className="flex-1 min-w-0">
          <h3 className="font-headline-sm text-headline-sm text-text-main truncate mb-1">{book.title}</h3>
          <p className="font-body-sm text-body-sm text-on-surface-variant">{book.author}</p>
        </div>

        <span className={`font-label-sm text-label-sm flex-shrink-0 ${book.availableCopies > 0 ? 'text-status-available' : 'text-status-borrowed'}`}>
          {book.availableCopies > 0 ? t('available') : t('borrowed', { count: totalLoans })}
        </span>
      </div>
    </Link>
  );
}
