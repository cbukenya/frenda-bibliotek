'use client';

import { useEffect, useState } from 'react';
import Link from 'next/link';
import { useTranslations } from 'next-intl';
import { getMyLoans, type Loan, formatDate } from '@/lib/api';
import LoanCard from '@/components/LoanCard';

export default function LoansPage() {
  const t = useTranslations('loans');
  const [loans, setLoans] = useState<Loan[]>([]);
  const [loading, setLoading] = useState(true);

  const load = () => {
    setLoading(true);
    getMyLoans().then(setLoans).finally(() => setLoading(false));
  };

  useEffect(() => {
    load();
    window.addEventListener('frenda_user_changed', load);
    return () => window.removeEventListener('frenda_user_changed', load);
  }, []);

  const active = loans.filter(l => !l.returnedAt);
  const history = loans.filter(l => l.returnedAt);

  return (
    <div className="page--dark">
      <div className="container" style={{ paddingTop: '2.5rem' }}>
        <div className="page-header">
          <h1 className="page-title">{t('title')}</h1>
          <p className="page-subtitle">{t('subtitle')}</p>
        </div>

        <div className="section-header">
          <h2 className="section-title section-title--light">
            {t('activeLoans')}
            {active.length > 0 && (
              <span className="count-badge">{t('bookCount', { count: active.length })}</span>
            )}
          </h2>
        </div>

        {loading ? (
          <div className="loan-grid">
            {Array.from({ length: 3 }).map((_, i) => (
              <div key={i} style={{ borderRadius: 12, overflow: 'hidden', background: 'var(--dark-card)', border: '1px solid var(--dark-border)' }}>
                <div className="skeleton" style={{ aspectRatio: '16/9' }} />
                <div style={{ padding: '1rem', display: 'flex', flexDirection: 'column', gap: '.5rem' }}>
                  <div className="skeleton" style={{ height: 16, borderRadius: 4 }} />
                  <div className="skeleton" style={{ height: 12, width: '50%', borderRadius: 4 }} />
                </div>
              </div>
            ))}
          </div>
        ) : active.length === 0 ? (
          <div className="empty-state" style={{ color: 'var(--gray-400)' }}>
            <div className="empty-state__icon">📭</div>
            <div className="empty-state__title" style={{ color: 'var(--gray-300)' }}>{t('noActiveTitle')}</div>
            <p>
              {t('noActiveHint')}{' '}
              <Link href="/" style={{ color: 'var(--navy-light)' }}>{t('browseLink')}</Link>
            </p>
          </div>
        ) : (
          <div className="loan-grid">
            {active.map(loan => (
              <LoanCard key={loan.id} loan={loan} onReturned={load} />
            ))}
          </div>
        )}

        {history.length > 0 && (
          <>
            <div className="section-header" style={{ marginTop: '2rem' }}>
              <h2 className="section-title section-title--light">{t('history')}</h2>
              <span className="section-link section-link--light">{t('historyCount', { count: history.length })}</span>
            </div>

            <div className="history-section">
              <table className="history-table">
                <thead>
                  <tr>
                    <th>{t('colBookTitle')}</th>
                    <th>{t('colAuthor')}</th>
                    <th>{t('colBorrowed')}</th>
                    <th>{t('colReturned')}</th>
                    <th>{t('colStatus')}</th>
                  </tr>
                </thead>
                <tbody>
                  {history.map(loan => {
                    const due = new Date(loan.borrowedAt);
                    due.setDate(due.getDate() + 14);
                    const late = new Date(loan.returnedAt!) > due;

                    return (
                      <tr key={loan.id}>
                        <td>
                          <div className="history-book-cell">
                            <div style={{ width: 36, height: 48, display: 'flex', alignItems: 'center', justifyContent: 'center', background: '#252840', borderRadius: 3, fontSize: '1.2rem', flexShrink: 0 }}>
                              {loan.coverUrl
                                ? <img src={loan.coverUrl} alt={loan.bookTitle} className="history-thumb" />
                                : '📚'
                              }
                            </div>
                            {loan.bookTitle}
                          </div>
                        </td>
                        <td style={{ color: 'var(--gray-400)' }}>{loan.bookAuthor}</td>
                        <td>{formatDate(loan.borrowedAt)}</td>
                        <td>{formatDate(loan.returnedAt!)}</td>
                        <td>
                          <span className={`book-card__badge ${late ? 'badge--late' : 'badge--returned'}`} style={{ position: 'static', display: 'inline-block' }}>
                            {late ? t('statusLate') : t('statusOnTime')}
                          </span>
                        </td>
                      </tr>
                    );
                  })}
                </tbody>
              </table>
            </div>
          </>
        )}
      </div>
    </div>
  );
}
