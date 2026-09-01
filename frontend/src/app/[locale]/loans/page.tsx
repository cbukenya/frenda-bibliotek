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

  const active = loans.filter(l => !l.returnedAt)
    .sort((a, b) => new Date(b.borrowedAt).getTime() - new Date(a.borrowedAt).getTime());
  const history = loans.filter(l => l.returnedAt)
    .sort((a, b) => new Date(b.returnedAt!).getTime() - new Date(a.returnedAt!).getTime());

  return (
    <div className="pt-24 pb-20 md:pb-8">
      <main className="max-w-container-max mx-auto w-full px-margin-mobile md:px-margin-desktop py-8">

        {/* Header */}
        <header className="mb-12">
          <h1 className="font-headline-xl text-headline-xl text-primary mb-2">{t('title')}</h1>
          <p className="font-body-lg text-body-lg text-on-surface-variant">{t('subtitle')}</p>
        </header>

        {/* Active loans */}
        <section className="mb-16">
          <div className="flex items-center justify-between mb-6">
            <h2 className="font-headline-md text-headline-md">{t('activeLoans')}</h2>
            {active.length > 0 && (
              <span className="bg-primary-container text-on-primary-container font-label-sm text-label-sm px-3 py-1 rounded-full">
                {t('bookCount', { count: active.length })}
              </span>
            )}
          </div>

          {loading ? (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-gutter">
              {Array.from({ length: 3 }).map((_, i) => (
                <div key={i} className="rounded-xl overflow-hidden bg-surface-container-lowest shadow-sm animate-pulse">
                  <div className="h-48 bg-surface-container-high" />
                  <div className="p-gutter space-y-3">
                    <div className="h-4 bg-surface-container-high rounded w-3/4" />
                    <div className="h-3 bg-surface-container-high rounded w-1/2" />
                    <div className="h-10 bg-surface-container-high rounded mt-4" />
                  </div>
                </div>
              ))}
            </div>
          ) : active.length === 0 ? (
            <div className="flex flex-col items-center justify-center py-16 text-on-surface-variant">
              <span className="material-symbols-outlined text-6xl mb-4 text-outline">inbox</span>
              <p className="font-headline-sm text-headline-sm mb-2">{t('noActiveTitle')}</p>
              <p className="font-body-md text-body-md">
                {t('noActiveHint')}{' '}
                <Link href="/" className="text-primary font-label-md">{t('browseLink')}</Link>
              </p>
            </div>
          ) : (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-gutter">
              {active.map(loan => (
                <LoanCard key={loan.id} loan={loan} onReturned={load} />
              ))}
            </div>
          )}
        </section>

        {/* Lending history */}
        {history.length > 0 && (
          <section>
            <h2 className="font-headline-md text-headline-md text-text-main mb-6">{t('history')}</h2>
            <div className="bg-surface-container-lowest rounded-xl shadow-[0_4px_20px_rgba(71,80,144,0.08)] overflow-hidden">
              <div className="overflow-x-auto">
                <table className="w-full text-left border-collapse">
                  <thead>
                    <tr className="bg-surface-container-low text-on-surface-variant font-label-sm text-label-sm border-b border-outline-variant/30">
                      <th className="py-4 px-6">{t('colBookTitle')}</th>
                      <th className="py-4 px-6 hidden sm:table-cell">{t('colAuthor')}</th>
                      <th className="py-4 px-6 hidden md:table-cell">{t('colBorrowed')}</th>
                      <th className="py-4 px-6">{t('colReturned')}</th>
                      <th className="py-4 px-6 text-right">{t('colStatus')}</th>
                    </tr>
                  </thead>
                  <tbody className="font-body-sm text-body-sm divide-y divide-outline-variant/20">
                    {history.map(loan => {
                      const due = new Date(loan.borrowedAt);
                      due.setDate(due.getDate() + 14);
                      const late = new Date(loan.returnedAt!) > due;
                      return (
                        <tr key={loan.id} className="hover:bg-surface-container-low/50 transition-colors">
                          <td className="py-4 px-6 font-medium">{loan.bookTitle}</td>
                          <td className="py-4 px-6 text-on-surface-variant hidden sm:table-cell">{loan.bookAuthor}</td>
                          <td className="py-4 px-6 text-on-surface-variant hidden md:table-cell">{formatDate(loan.borrowedAt)}</td>
                          <td className="py-4 px-6 text-on-surface-variant">{formatDate(loan.returnedAt!)}</td>
                          <td className="py-4 px-6 text-right">
                            <span className={`px-2 py-1 rounded-full text-[10px] ${late ? 'bg-error/10 text-error' : 'bg-status-available/10 text-status-available'}`}>
                              {late ? t('statusLate') : t('statusOnTime')}
                            </span>
                          </td>
                        </tr>
                      );
                    })}
                  </tbody>
                </table>
              </div>
            </div>
          </section>
        )}
      </main>
    </div>
  );
}
