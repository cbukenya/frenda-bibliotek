'use client';

import { useState } from 'react';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { useTranslations } from 'next-intl';
import { login } from '@/lib/api';

export default function LoginPage() {
  const t = useTranslations('login');
  const router = useRouter();
  const [showPassword, setShowPassword] = useState(false);
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      await login(email, password);
      router.push('/');
    } catch (err) {
      setError((err as Error).message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-[#EEF1FF] via-background to-[#E8F4F8] px-4">
      <div className="w-full max-w-md bg-surface-container-lowest rounded-2xl shadow-[0_8px_30px_rgba(71,80,144,0.12)] p-10">
        {/* Icon */}
        <div className="flex justify-center mb-6">
          <div className="w-12 h-12 rounded-full bg-primary flex items-center justify-center">
            <span className="material-symbols-outlined text-on-primary text-2xl">menu_book</span>
          </div>
        </div>

        {/* Title */}
        <h1 className="font-headline-lg text-headline-lg text-primary text-center mb-2">
          {t('title')}
        </h1>
        <p className="font-body-md text-body-md text-on-surface-variant text-center mb-8">
          {t('subtitle')}
        </p>

        {error && (
          <div className="mb-4 p-3 rounded-lg bg-red-50 text-red-600 text-sm text-center">
            {error}
          </div>
        )}

        <form onSubmit={handleSubmit} className="flex flex-col gap-5">
          {/* Email */}
          <div>
            <label className="block font-label-md text-label-md text-on-surface mb-1.5">
              {t('emailLabel')}
            </label>
            <div className="relative">
              <span className="material-symbols-outlined absolute left-3 top-3 text-on-surface-variant text-[20px]">mail</span>
              <input
                type="email"
                value={email}
                onChange={e => setEmail(e.target.value)}
                placeholder={t('emailPlaceholder')}
                required
                className="w-full bg-surface-container-low border-none rounded-lg py-3 pl-10 pr-4 font-body-md text-body-md text-on-surface placeholder:text-outline focus:ring-2 focus:ring-primary outline-none transition-all"
              />
            </div>
          </div>

          {/* Password */}
          <div>
            <label className="block font-label-md text-label-md text-on-surface mb-1.5">
              {t('passwordLabel')}
            </label>
            <div className="relative">
              <span className="material-symbols-outlined absolute left-3 top-3 text-on-surface-variant text-[20px]">lock</span>
              <input
                type={showPassword ? 'text' : 'password'}
                value={password}
                onChange={e => setPassword(e.target.value)}
                placeholder={t('passwordPlaceholder')}
                required
                className="w-full bg-surface-container-low border-none rounded-lg py-3 pl-10 pr-12 font-body-md text-body-md text-on-surface placeholder:text-outline focus:ring-2 focus:ring-primary outline-none transition-all"
              />
              <button
                type="button"
                onClick={() => setShowPassword(v => !v)}
                className="absolute right-3 top-3 text-on-surface-variant hover:text-primary transition-colors"
              >
                <span className="material-symbols-outlined text-[20px]">
                  {showPassword ? 'visibility_off' : 'visibility'}
                </span>
              </button>
            </div>
          </div>

          {/* Submit */}
          <button
            type="submit"
            disabled={loading}
            className="w-full bg-primary text-on-primary font-label-md text-label-md py-3.5 rounded-lg hover:opacity-90 transition-opacity mt-2 disabled:opacity-60"
          >
            {loading ? t('loggingIn') : t('submit')}
          </button>
        </form>

        {/* Register link */}
        <p className="text-center font-body-sm text-body-sm text-on-surface-variant mt-6">
          {t('noAccount')}{' '}
          <Link href="/register" className="text-primary font-label-md hover:underline">
            {t('createAccount')}
          </Link>
        </p>
      </div>
    </div>
  );
}
