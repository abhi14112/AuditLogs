import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { Package, History, LogOut, Menu, X } from 'lucide-react';
import { useState } from 'react';

const Layout = ({ children }) => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);

  const [auditDropdownOpen, setAuditDropdownOpen] = useState(false);

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <div className="min-h-screen bg-gray-50">
      {/* Header */}
      <header className="bg-white shadow-sm border-b border-gray-200 sticky top-0 z-50">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center h-16">
            <div className="flex items-center gap-3">
              <div className="w-10 h-10 bg-gradient-to-br from-primary-600 to-primary-700 rounded-lg flex items-center justify-center">
                <Package className="text-white" size={24} />
              </div>
              <h1 className="text-xl font-bold text-gray-900">Inventory Manager</h1>
            </div>

            {/* Desktop Navigation */}
            <nav className="hidden md:flex items-center gap-6">
              <Link
                to="/"
                className="flex items-center gap-2 text-gray-700 hover:text-primary-600 font-medium transition-colors"
              >
                <Package size={20} />
                Products
              </Link>
              <div className="relative">
                <button
                  onClick={() => setAuditDropdownOpen(!auditDropdownOpen)}
                  className="flex items-center gap-2 text-gray-700 hover:text-primary-600 font-medium transition-colors"
                >
                  <History size={20} />
                  Audit Logs
                  <svg
                    className={`w-4 h-4 transition-transform ${auditDropdownOpen ? "rotate-180" : ""}`}
                    fill="none"
                    stroke="currentColor"
                    strokeWidth="2"
                    viewBox="0 0 24 24"
                  >
                    <path strokeLinecap="round" strokeLinejoin="round" d="M19 9l-7 7-7-7" />
                  </svg>
                </button>

                {auditDropdownOpen && (
                  <div className="absolute left-0 mt-2 w-48 bg-white border border-gray-200 rounded-lg shadow-lg z-50">
                    
                    <Link
                      to="/audit-logs"
                      onClick={() => setAuditDropdownOpen(false)}
                      className="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 rounded-t-lg"
                    >
                      Audit Logs v1
                    </Link>

                    <Link
                      to="/audit-logs-v2"
                      onClick={() => setAuditDropdownOpen(false)}
                      className="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 rounded-b-lg"
                    >
                      Audit Logs v2
                    </Link>

                  </div>
                )}
              </div>

              <div className="flex items-center gap-4 ml-4 pl-4 border-l border-gray-200">
                <span className="text-gray-700">👤 {user?.username}</span>
                <button onClick={handleLogout} className="btn-secondary flex items-center gap-2">
                  <LogOut size={18} />
                  Logout
                </button>
              </div>
            </nav>

            {/* Mobile Menu Button */}
            <button
              className="md:hidden p-2"
              onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
            >
              {isMobileMenuOpen ? <X size={24} /> : <Menu size={24} />}
            </button>
          </div>

          {/* Mobile Navigation */}
          {isMobileMenuOpen && (
            <div className="md:hidden py-4 border-t border-gray-200">
              <nav className="flex flex-col gap-4">
                <Link
                  to="/"
                  className="flex items-center gap-2 text-gray-700 hover:text-primary-600 font-medium"
                  onClick={() => setIsMobileMenuOpen(false)}
                >
                  <Package size={20} />
                  Products
                </Link>
                <Link
                  to="/audit-logs"
                  className="flex items-center gap-2 text-gray-700 hover:text-primary-600 font-medium"
                  onClick={() => setIsMobileMenuOpen(false)}
                >
                  <History size={20} />
                  Audit Logs
                </Link>
                <div className="pt-4 border-t border-gray-200">
                  <p className="text-gray-700 mb-3">👤 {user?.username}</p>
                  <button onClick={handleLogout} className="btn-secondary flex items-center gap-2">
                    <LogOut size={18} />
                    Logout
                  </button>
                </div>
              </nav>
            </div>
          )}
        </div>
      </header>

      {/* Main Content */}
      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {children}
      </main>
    </div>
  );
};

export default Layout;
