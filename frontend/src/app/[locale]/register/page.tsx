'use client';

import { useState } from 'react';
import Link from 'next/link';
import { useTranslations } from 'next-intl';

export default function RegisterPage() {
  const t = useTranslations('register');
  const [showPassword, setShowPassword] = useState(false);

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-[#EEF1FF] via-background to-[#E8F4F8] px-4">
      <div className="w-full max-w-md bg-surface-container-lowest rounded-2xl shadow-[0_8px_30px_rgba(71,80,144,0.12)] p-10">
        {/* Title */}
        <h1 className="font-headline-lg text-headline-lg text-primary text-center mb-2">
          {t('title')}
        </h1>
        <p className="font-body-md text-body-md text-on-surface-variant text-center mb-8">
          {t('subtitle')}
        </p>

        <form onSubmit={e => e.preventDefault()} className="flex flex-col gap-5">
          {/* Full name */}
          <div>
            <label className="block font-label-md text-label-md text-on-surface mb-1.5">
              {t('nameLabel')}
            </label>
            <input
              type="text"
              placeholder={t('namePlaceholder')}
              className="w-full bg-surface-container-low border-none rounded-lg py-3 px-4 font-body-md text-body-md text-on-surface placeholder:text-outline focus:ring-2 focus:ring-primary outline-none transition-all"
            />
          </div>

          {/* Email */}
          <div>
            <label className="block font-label-md text-label-md text-on-surface mb-1.5">
              {t('emailLabel')}
            </label>
            <input
              type="email"
              placeholder={t('emailPlaceholder')}
              className="w-full bg-surface-container-low border-none rounded-lg py-3 px-4 font-body-md text-body-md text-on-surface placeholder:text-outline focus:ring-2 focus:ring-primary outline-none transition-all"
            />
          </div>

          {/* Password */}
          <div>
            <label className="block font-label-md text-label-md text-on-surface mb-1.5">
              {t('passwordLabel')}
            </label>
            <input
              type={showPassword ? 'text' : 'password'}
              placeholder={t('passwordPlaceholder')}
              className="w-full bg-surface-container-low border-none rounded-lg py-3 px-4 font-body-md text-body-md text-on-surface placeholder:text-outline focus:ring-2 focus:ring-primary outline-none transition-all"
            />
          </div>

          {/* Confirm password */}
          <div>
            <label className="block font-label-md text-label-md text-on-surface mb-1.5">
              {t('confirmPasswordLabel')}
            </label>
            <input
              type={showPassword ? 'text' : 'password'}
              placeholder={t('passwordPlaceholder')}
              className="w-full bg-surface-container-low border-none rounded-lg py-3 px-4 font-body-md text-body-md text-on-surface placeholder:text-outline focus:ring-2 focus:ring-primary outline-none transition-all"
            />
          </div>

          {/* Submit */}
          <button
            type="submit"
            className="w-full bg-primary text-on-primary font-label-md text-label-md py-3.5 rounded-lg hover:opacity-90 transition-opacity mt-2 flex items-center justify-center gap-2"
          >
            {t('submit')}
            <span className="material-symbols-outlined text-[20px]">arrow_forward</span>
          </button>
        </form>

        {/* Login link */}
        <p className="text-center font-body-sm text-body-sm text-on-surface-variant mt-6">
          {t('hasAccount')}{' '}
          <Link href="/login" className="text-primary font-label-md hover:underline">
            {t('loginLink')}
          </Link>
        </p>
      </div>
    </div>
  );
}
