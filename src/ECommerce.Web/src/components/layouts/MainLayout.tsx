import { Outlet, Link } from 'react-router-dom';

export function MainLayout() {
    return (
        <div className="min-h-screen bg-gray-50 flex flex-col">
            <header className="bg-white border-b sticky top-0 z-10 w-full px-6 py-4 flex items-center justify-between">
                <div className="font-bold text-xl">E-Commerce</div>
                <nav className="flex gap-4">
                    <Link to="/" className="text-sm font-medium hover:text-primary">Home</Link>
                    <Link to="/products" className="text-sm font-medium hover:text-primary">Products</Link>
                    <Link to="/basket" className="text-sm font-medium hover:text-primary">Basket</Link>
                    <Link to="/login" className="text-sm font-medium hover:text-primary">Login</Link>
                </nav>
            </header>
            <main className="flex-1 max-w-7xl mx-auto w-full p-6">
                <Outlet />
            </main>
        </div>
    );
}
