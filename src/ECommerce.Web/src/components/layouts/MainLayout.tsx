import { Outlet, Link } from 'react-router-dom';
import { useAuthStore } from '@/store/authStore';
import { useLogout } from '@/features/auth/hooks/useLogout';

export function MainLayout() {
    const { isAuthenticated, user, isAuthLoading } = useAuthStore();
    const { mutate: logout, isPending: isLoggingOut } = useLogout();

    return (
        <div className="min-h-screen bg-gray-50 flex flex-col">
            <header className="bg-white border-b sticky top-0 z-10 w-full px-6 py-4 flex items-center justify-between">
                <div className="font-bold text-xl">
                    <Link to="/">E-Commerce</Link>
                </div>
                <nav className="flex items-center gap-4">
                    <Link to="/" className="text-sm font-medium hover:text-primary">Home</Link>
                    <Link to="/products" className="text-sm font-medium hover:text-primary">Products</Link>
                    <Link to="/basket" className="text-sm font-medium hover:text-primary">Basket</Link>

                    {!isAuthLoading && (
                        <div className="flex items-center gap-4 ml-4 border-l pl-4 border-gray-200">
                            {isAuthenticated ? (
                                <>
                                    <span className="text-sm text-gray-600">Hi, {user?.fullName}</span>
                                    {user?.role === 'Admin' && (
                                        <Link to="/admin" className="text-sm font-medium hover:text-primary text-blue-600">Admin Panel</Link>
                                    )}
                                    <button
                                        onClick={() => logout()}
                                        disabled={isLoggingOut}
                                        className="text-sm font-medium text-red-600 hover:text-red-500 disabled:opacity-50 transition-colors"
                                    >
                                        Logout
                                    </button>
                                </>
                            ) : (
                                <>
                                    <Link to="/login" className="text-sm font-medium hover:text-primary">Login</Link>
                                    <Link to="/register" className="text-sm font-medium hover:text-primary text-blue-600">Register</Link>
                                </>
                            )}
                        </div>
                    )}
                </nav>
            </header>
            <main className="flex-1 max-w-7xl mx-auto w-full p-6">
                <Outlet />
            </main>
        </div>
    );
}
