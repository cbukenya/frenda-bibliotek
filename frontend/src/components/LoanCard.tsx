'use client';

import { useState } from 'react';
import { useTranslations } from 'next-intl';
import { type Loan, returnLoan, daysUntilDue, formatDate } from '@/lib/api';

interface Props {
  loan: Loan;
  onReturned?: () => void;
}

export default function LoanCard({ loan, onReturned }: Props) {
  const t = useTranslations('loanCard');
  const [loading, setLoading] = useState(false);
  const days = daysUntilDue(loan.borrowedAt);

  const badgeColor = days > 5 ? 'text-status-available' : days >= 0 ? 'text-status-borrowed' : 'text-error';
  const badgeLabel = days >= 0 ? t('dueIn', { days }) : t('overdue', { days: Math.abs(days) });

  const handleReturn = async () => {
    setLoading(true);
    try { await returnLoan(loan.id); onReturned?.(); }
    catch (err) { alert((err as Error).message); }
    finally { setLoading(false); }
  };

  return (
    <div className="glass-card rounded-xl overflow-hidden shadow-[0_4px_20px_rgba(71,80,144,0.08)] flex flex-col bg-surface-container-lowest">
      {/* Cover */}
      <div className="relative h-48 bg-surface-container-low">
        {loan.coverUrl ? (
          <img src={loan.coverUrl} alt={loan.bookTitle} className="w-full h-full object-cover" />
        ) : (
          <div className="w-full h-full flex items-center justify-center text-5xl bg-surface-container">
            📚
          </div>
        )}
        <div className={`absolute top-3 right-3 bg-white/80 backdrop-blur-sm px-3 py-1 rounded-full font-label-sm shadow-sm flex items-center gap-1 ${badgeColor}`}>
          <span className="material-symbols-outlined text-sm">timer</span>
          {badgeLabel}
        </div>
      </div>

      {/* Body */}
      <div className="p-gutter flex flex-col flex-grow">
        <div className="mb-4 flex-grow">
          <h3 className="font-headline-sm text-headline-sm text-text-main line-clamp-1">{loan.bookTitle}</h3>
          <p className="font-body-sm text-body-sm text-on-surface-variant">{loan.bookAuthor}</p>
          <div className="mt-2 flex flex-col gap-1 text-body-sm text-on-surface-variant">
            <span>{t('borrowedLabel')} <strong className="text-on-surface">{formatDate(loan.borrowedAt)}</strong></span>
          </div>
        </div>

        <button
          onClick={handleReturn}
          disabled={loading}
          className="w-full bg-primary-container text-on-primary-container py-2 rounded-lg font-label-md text-label-md flex justify-center items-center gap-2 hover:opacity-90 transition-opacity"
        >
          {loading ? t('returning') : t('returnBook')}
          {!loading && <span className="material-symbols-outlined text-[18px]">keyboard_return</span>}
        </button>
      </div>
    </div>
  );
}
