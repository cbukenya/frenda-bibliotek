'use client';

import { useState } from 'react';
import { type Loan, returnLoan, daysUntilDue, formatDate, genreEmoji } from '@/lib/api';

interface Props {
  loan: Loan;
  onReturned?: () => void;
}

export default function LoanCard({ loan, onReturned }: Props) {
  const [loading, setLoading] = useState(false);
  const days = daysUntilDue(loan.borrowedAt);
  const dueDate = new Date(loan.borrowedAt);
  dueDate.setDate(dueDate.getDate() + 14);

  const badgeClass = days > 5 ? 'badge--due-ok' : days >= 0 ? 'badge--due-warn' : 'badge--due-late';
  const badgeLabel = days > 5
    ? `Due in ${days}d`
    : days >= 0
      ? `Due in ${days}d`
      : `${Math.abs(days)}d overdue`;

  const handleReturn = async () => {
    setLoading(true);
    try {
      await returnLoan(loan.id);
      onReturned?.();
    } catch (err) {
      alert((err as Error).message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="loan-card">
      <div className="loan-card__cover">
        {loan.coverUrl ? (
          <img src={loan.coverUrl} alt={loan.bookTitle} />
        ) : (
          <div style={{ width: '100%', height: '100%', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: '2.5rem' }}>
            📚
          </div>
        )}
        <span className={`loan-card__due-badge ${badgeClass}`}>{badgeLabel}</span>
      </div>

      <div className="loan-card__body">
        <div className="loan-card__title">{loan.bookTitle}</div>
        <div className="loan-card__author">{loan.bookAuthor}</div>
        <div className="loan-card__dates">
          <span>
            <span>Borrowed:</span>
            <strong>{formatDate(loan.borrowedAt)}</strong>
          </span>
          <span>
            <span>Due Date:</span>
            <strong className={days < 0 ? 'overdue' : ''}>
              {dueDate.toLocaleDateString('en-GB', { day: 'numeric', month: 'short', year: 'numeric' })}
            </strong>
          </span>
        </div>
      </div>

      <div className="loan-card__actions">
        <button
          className="btn btn--primary btn--sm btn--full"
          onClick={handleReturn}
          disabled={loading}
        >
          {loading ? 'Returning…' : '↩ Return Book'}
        </button>
      </div>
    </div>
  );
}
