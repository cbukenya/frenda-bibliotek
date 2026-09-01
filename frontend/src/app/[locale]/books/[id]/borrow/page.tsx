'use client';

import { useState } from 'react';
import { useParams, useRouter } from 'next/navigation';
import { useTranslations } from 'next-intl';
import { useEffect } from 'react';
import { getBook, borrowBook, isLoggedIn, type BookDetail, genreEmoji } from '@/lib/api';

export default function BorrowBookPage() {
  const t = useTranslations('borrowPage');
  const params = useParams();
  const router = useRouter();
  const bookId = Number(params.id);

  const [book, setBook] = useState<BookDetail | null>(null);
  const [loading, setLoading] = useState(true);
  const [borrowing, setBorrowing] = useState(false);
  const [borrowed, setBorrowed] = useState(false);
  const [error, setError] = useState('');

  // Default return date: 14 days from now
  const defaultReturn = new Date();
  defaultReturn.setDate(defaultReturn.getDate() + 14);
  const [returnDate, setReturnDate] = useState(defaultReturn.toISOString().split('T')[0]);

  useEffect(() => {
    if (!isLoggedIn()) {
      window.location.href = '/login';
      return;
    }
    getBook(bookId).then(setBook).catch(() => setError('Book not found')).finally(() => setLoading(false));
  }, [bookId]);

  const handleBorrow = async () => {
    if (!book || borrowing || borrowed) return;
    setBorrowing(true);
    setError('');
    try {
      await borrowBook(book.isbn, returnDate);
      setBorrowed(true);
    } catch (err) {
      setError((err as Error).message);
    } finally {
      setBorrowing(false);
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-primary" />
      </div>
    );
  }

  if (!book) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <p className="text-on-surface-variant">{error || 'Book not found'}</p>
      </div>
    );
  }

  const available = book.availableCopies > 0;

  return (
    <div className="min-h-screen bg-gradient-to-b from-background to-surface pt-24 md:pt-28 pb-32 md:pb-16">
      <main className="px-margin-mobile md:px-margin-desktop max-w-3xl mx-auto">
        {/* Back */}
        <button
          onClick={() => router.back()}
          className="flex items-center gap-1 text-on-surface-variant hover:text-primary mb-8 transition-colors"
        >
          <span className="material-symbols-outlined text-sm">arrow_back</span>
          <span className="font-label-md text-label-md">{t('back')}</span>
        </button>

        <div className="bg-surface-container-lowest rounded-2xl shadow-[0_8px_30px_rgba(71,80,144,0.12)] overflow-hidden">
          {/* Book summary header */}
          <div className="flex gap-6 p-8 border-b border-outline-variant/20">
            <div className="w-24 h-36 rounded-lg overflow-hidden bg-surface-container-low flex-shrink-0">
              {book.coverUrl ? (
                <img src={book.coverUrl} alt={book.title} className="w-full h-full object-cover" />
              ) : (
                <div className="w-full h-full flex items-center justify-center text-3xl bg-surface-container">
                  {genreEmoji(book.genre)}
                </div>
              )}
            </div>
            <div className="flex flex-col justify-center">
              <h1 className="font-headline-md text-headline-md text-on-background mb-1">{book.title}</h1>
              <p className="font-body-md text-body-md text-on-surface-variant mb-2">{book.author}</p>
              <div className="flex items-center gap-1.5">
                <span className={`w-2 h-2 rounded-full ${available ? 'bg-status-available' : 'bg-status-borrowed'}`} />
                <span className={`font-label-sm text-label-sm ${available ? 'text-status-available' : 'text-status-borrowed'}`}>
                  {available ? t('available', { count: book.availableCopies }) : t('unavailable')}
                </span>
              </div>
            </div>
          </div>

          {/* Loan form */}
          <div className="p-8">
            {borrowed ? (
              <div className="text-center py-8">
                <div className="w-16 h-16 rounded-full bg-status-available/10 flex items-center justify-center mx-auto mb-4">
                  <span className="material-symbols-outlined text-status-available text-3xl">check_circle</span>
                </div>
                <h2 className="font-headline-sm text-headline-sm text-on-background mb-2">{t('successTitle')}</h2>
                <p className="font-body-md text-body-md text-on-surface-variant mb-6">{t('successMessage')}</p>
                <button
                  onClick={() => window.location.href = '/loans'}
                  className="bg-primary text-on-primary font-label-md text-label-md px-8 py-3 rounded-lg hover:opacity-90 transition-opacity"
                >
                  {t('viewLoans')}
                </button>
              </div>
            ) : (
              <>
                <h2 className="font-headline-sm text-headline-sm text-on-background mb-6">{t('loanDetails')}</h2>

                {/* Return date picker */}
                <div className="mb-6">
                  <label className="block font-label-md text-label-md text-on-surface mb-2">
                    {t('returnDateLabel')}
                  </label>
                  <input
                    type="date"
                    value={returnDate}
                    onChange={e => setReturnDate(e.target.value)}
                    min={new Date().toISOString().split('T')[0]}
                    className="w-full bg-surface-container-low border-none rounded-lg py-3 px-4 font-body-md text-body-md text-on-surface focus:ring-2 focus:ring-primary outline-none transition-all"
                  />
                  <p className="font-body-sm text-body-sm text-on-surface-variant mt-1.5">
                    {t('returnDateHint')}
                  </p>
                </div>

                {/* Loan summary */}
                <div className="bg-surface-container-low rounded-xl p-5 mb-6">
                  <div className="flex justify-between items-center mb-3">
                    <span className="font-label-sm text-label-sm text-on-surface-variant">{t('bookTitle')}</span>
                    <span className="font-body-sm text-body-sm text-on-surface font-medium">{book.title}</span>
                  </div>
                  <div className="flex justify-between items-center mb-3">
                    <span className="font-label-sm text-label-sm text-on-surface-variant">{t('loanDate')}</span>
                    <span className="font-body-sm text-body-sm text-on-surface font-medium">{new Date().toLocaleDateString()}</span>
                  </div>
                  <div className="flex justify-between items-center">
                    <span className="font-label-sm text-label-sm text-on-surface-variant">{t('plannedReturn')}</span>
                    <span className="font-body-sm text-body-sm text-on-surface font-medium">{new Date(returnDate).toLocaleDateString()}</span>
                  </div>
                </div>

                {error && (
                  <div className="mb-4 p-3 rounded-lg bg-red-50 text-red-600 text-sm text-center">
                    {error}
                  </div>
                )}

                {/* Actions */}
                <div className="flex gap-4">
                  <button
                    onClick={handleBorrow}
                    disabled={!available || borrowing}
                    className="flex-1 bg-primary text-on-primary font-label-md text-label-md py-3.5 rounded-lg hover:opacity-90 transition-opacity flex items-center justify-center gap-2 disabled:opacity-60 disabled:cursor-not-allowed"
                  >
                    <span className="material-symbols-outlined text-[20px]">library_add</span>
                    {borrowing ? t('borrowing') : t('confirmBorrow')}
                  </button>
                  <button
                    onClick={() => router.back()}
                    className="px-6 py-3.5 border-2 border-outline-variant text-on-surface-variant font-label-md text-label-md rounded-lg hover:bg-surface-container-low transition-colors"
                  >
                    {t('cancel')}
                  </button>
                </div>
              </>
            )}
          </div>
        </div>
      </main>
    </div>
  );
}
