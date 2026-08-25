'use client';

import { useEffect, useState } from 'react';
import { getMyLoans, type Loan, formatDate, genreEmoji } from '@/lib/api';
import LoanCard from '@/components/LoanCard';

export default function LoansPage() {
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
          <h1 className="page-title">My Loans</h1>
          <p className="page-subtitle">Manage your current reading and view your history.</p>
        </div>

        {/* Active loans */}
        <div className="section-header">
          <h2 className="section-title section-title--light">
            Active Loans
            {active.length > 0 && <span className="count-badge">{active.length} books</span>}
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
            <div className="empty-state__title" style={{ color: 'var(--gray-300)' }}>No active loans</div>
            <p>Head to <a href="/" style={{ color: 'var(--navy-light)' }}>Browse</a> to borrow a book.</p>
          </div>
        ) : (
          <div className="loan-grid">
            {active.map(loan => (
              <LoanCard key={loan.id} loan={loan} onReturned={load} />
            ))}
          </div>
        )}

        {/* Lending history */}
        {history.length > 0 && (
          <>
            <div className="section-header" style={{ marginTop: '2rem' }}>
              <h2 className="section-title section-title--light">Lending History</h2>
              <span className="section-link section-link--light">{history.length} books</span>
            </div>

            <div className="history-section">
              <table className="history-table">
                <thead>
                  <tr>
                    <th>Book Title</th>
                    <th>Author</th>
                    <th>Borrowed</th>
                    <th>Returned</th>
                    <th>Status</th>
                  </tr>
                </thead>
                <tbody>
                  {history.map(loan => {
                    const borrowed = new Date(loan.borrowedAt);
                    const returned = new Date(loan.returnedAt!);
                    const due = new Date(loan.borrowedAt);
                    due.setDate(due.getDate() + 14);
                    const late = returned > due;

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
                            {late ? 'Returned Late' : 'Returned On Time'}
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
