import { Outlet } from 'react-router-dom';
import Header from './Header';
import { useTranslation } from 'react-i18next';

interface LayoutProps {
  children?: React.ReactNode;
}

export default function Layout({ children }: LayoutProps) {
  const { t } = useTranslation();

  return (
    <div className="min-h-screen flex flex-col bg-white">
      <Header />
      
      <main className="flex-grow w-full max-w-[2520px] mx-auto xl:px-20 md:px-10 sm:px-2 px-4 py-8">
        {children || <Outlet />}
      </main>

      <footer className="w-full border-t border-gray-200 bg-gray-50 py-6 text-center text-sm text-foggy">
        <div className="container mx-auto px-6">
          &copy; {new Date().getFullYear()} {t('common.copyright')}
        </div>
      </footer>
    </div>
  );
}
