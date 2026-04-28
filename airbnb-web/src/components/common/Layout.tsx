import Header from './Header';

interface LayoutProps {
  children: React.ReactNode;
}

export default function Layout({ children }: LayoutProps) {
  return (
    <div className="min-h-screen flex flex-col bg-white">
      <Header />
      
      <main className="flex-grow container mx-auto px-6 md:px-12 py-8">
        {children}
      </main>

      <footer className="w-full border-t border-gray-200 bg-gray-50 py-6 text-center text-sm text-foggy">
        <div className="container mx-auto px-6">
          &copy; {new Date().getFullYear()} Airbnb Clone. Toàn quyền sở hữu.
        </div>
      </footer>
    </div>
  );
}
