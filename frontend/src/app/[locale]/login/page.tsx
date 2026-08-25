'use client';

import { useState } from 'react';
import Link from 'next/link';
import { useTranslations } from 'next-intl';

export default function LoginPage() {
  const t = useTranslations('login');
  const [showPassword, setShowPassword] = useState(false);

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

        <form onSubmit={e => e.preventDefault()} className="flex flex-col gap-5">
          {/* Email */}
          <div>
            <label className="block font-label-md text-label-md text-on-surface mb-1.5">
              {t('emailLabel')}
            </label>
            <div className="relative">
              <span className="material-symbols-outlined absolute left-3 top-3 text-on-surface-variant text-[20px]">mail</span>
              <input
                type="email"
                placeholder={t('emailPlaceholder')}
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
                placeholder={t('passwordPlaceholder')}
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

          {/* Remember / Forgot */}
          <div className="flex items-center justify-between">
            <label className="flex items-center gap-2 cursor-pointer">
              <input
                type="checkbox"
                className="w-4 h-4 rounded border-outline-variant text-primary focus:ring-primary"
              />
              <span className="font-body-sm text-body-sm text-on-surface-variant">{t('rememberMe')}</span>
            </label>
            <Link href="#" className="font-label-sm text-label-sm text-primary hover:underline">
              {t('forgotPassword')}
            </Link>
          </div>

          {/* Submit */}
          <button
            type="submit"
            className="w-full bg-primary text-on-primary font-label-md text-label-md py-3.5 rounded-lg hover:opacity-90 transition-opacity mt-2"
          >
            {t('submit')}
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
