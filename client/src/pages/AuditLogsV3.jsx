import React, { useState, useEffect, useCallback } from 'react';
import { useAuth } from '../context/AuthContext';
import Layout from '../components/Layout';
import AuditTimeline from '../components/auditv3/AuditTimeline';
import AuditFilters from '../components/auditv3/AuditFilters';
import AuditComparisonModal from '../components/auditv3/AuditComparisonModal';
import auditLogV3Service from '../services/auditLogV3Service';
import signalRV3Service from '../services/signalRV3Service';

/**
 * AuditLogsV3 Page - Enterprise-level Jira-style audit logging
 */
const AuditLogsV3 = () => {
  const { user } = useAuth();
  const [auditLogs, setAuditLogs] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [selectedLog, setSelectedLog] = useState(null);
  const [isModalOpen, setIsModalOpen] = useState(false);
  
  // Pagination
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [pageSize] = useState(20);
  
  // Filters
  const [filters, setFilters] = useState({
    searchTerm: '',
    entityType: '',
    action: '',
    severity: '',
    startDate: null,
    endDate: null,
    sortBy: 'createdAt',
    sortDescending: true
  });

  // Stats
  const [stats, setStats] = useState(null);
  const [connectionStatus, setConnectionStatus] = useState('Disconnected');

  // Fetch audit logs
  const fetchAuditLogs = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);

      const params = {
        page: currentPage,
        pageSize,
        ...filters,
        startDate: filters.startDate ? new Date(filters.startDate) : null,
        endDate: filters.endDate ? new Date(filters.endDate) : null
      };

      const response = await auditLogV3Service.getAuditLogs(params);
      
      setAuditLogs(response.items);
      setTotalPages(response.totalPages);
      setTotalCount(response.totalCount);
      setCurrentPage(response.page);
    } catch (err) {
      console.error('Error fetching audit logs:', err);
      setError('Failed to load audit logs. Please try again.');
    } finally {
      setLoading(false);
    }
  }, [currentPage, pageSize, filters]);

  // Fetch stats
  const fetchStats = useCallback(async () => {
    try {
      const statsData = await auditLogV3Service.getAuditLogStats(
        filters.startDate ? new Date(filters.startDate) : null,
        filters.endDate ? new Date(filters.endDate) : null
      );
      setStats(statsData);
    } catch (err) {
      console.error('Error fetching stats:', err);
    }
  }, [filters.startDate, filters.endDate]);

  // Initialize SignalR connection
  useEffect(() => {
    const initSignalR = async () => {
      try {
        await signalRV3Service.start();
        setConnectionStatus(signalRV3Service.getConnectionState());

        // Listen for new audit logs
        const unsubscribe = signalRV3Service.on('auditLogReceived', (newLog) => {
          console.log('New audit log received:', newLog);
          setAuditLogs(prev => [newLog, ...prev]);
          setTotalCount(prev => prev + 1);
          
          // Show notification
          if (Notification.permission === 'granted') {
            new Notification('New Audit Log', {
              body: newLog.description,
              icon: '/favicon.ico'
            });
          }
        });

        // Listen for connection status changes
        signalRV3Service.on('connected', () => setConnectionStatus('Connected'));
        signalRV3Service.on('reconnecting', () => setConnectionStatus('Reconnecting'));
        signalRV3Service.on('reconnected', () => setConnectionStatus('Connected'));
        signalRV3Service.on('connectionClosed', () => setConnectionStatus('Disconnected'));

        return () => {
          unsubscribe();
          signalRV3Service.stop();
        };
      } catch (err) {
        console.error('SignalR connection error:', err);
        setConnectionStatus('Error');
      }
    };

    initSignalR();
  }, []);

  // Fetch data on mount and when filters/page change
  useEffect(() => {
    fetchAuditLogs();
  }, [fetchAuditLogs]);

  useEffect(() => {
    fetchStats();
  }, [fetchStats]);

  // Request notification permission
  useEffect(() => {
    if (Notification.permission === 'default') {
      Notification.requestPermission();
    }
  }, []);

  const handleFilterChange = (newFilters) => {
    setFilters(newFilters);
    setCurrentPage(1); // Reset to first page when filters change
  };

  const handleResetFilters = () => {
    const resetFilters = {
      searchTerm: '',
      entityType: '',
      action: '',
      severity: '',
      startDate: null,
      endDate: null,
      sortBy: 'createdAt',
      sortDescending: true
    };
    setFilters(resetFilters);
    setCurrentPage(1);
  };

  const handleViewDetails = (log) => {
    setSelectedLog(log);
    setIsModalOpen(true);
  };

  const handleRefresh = () => {
    fetchAuditLogs();
    fetchStats();
  };

  return (
    <Layout>
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {/* Header */}
        <div className="mb-8">
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-3xl font-bold text-gray-900">Audit Logs V3</h1>
            <p className="text-gray-600 mt-1">Enterprise-level Jira-style audit logging system</p>
          </div>
          
          <div className="flex items-center gap-4">
            {/* Connection Status */}
            <div className="flex items-center gap-2">
              <div className={`w-3 h-3 rounded-full ${
                connectionStatus === 'Connected' ? 'bg-green-500' :
                connectionStatus === 'Reconnecting' ? 'bg-yellow-500' :
                'bg-red-500'
              }`}></div>
              <span className="text-sm text-gray-600">{connectionStatus}</span>
            </div>

            {/* Refresh Button */}
            <button
              onClick={handleRefresh}
              className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors flex items-center gap-2"
            >
              <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
              </svg>
              Refresh
            </button>
          </div>
        </div>
      </div>

      {/* Stats Cards */}
      {stats && (
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
          <div className="bg-white border border-gray-200 rounded-lg p-4 shadow-sm">
            <p className="text-sm text-gray-600 mb-1">Total Logs</p>
            <p className="text-2xl font-bold text-gray-900">{stats.totalLogs}</p>
          </div>
          
          <div className="bg-white border border-gray-200 rounded-lg p-4 shadow-sm">
            <p className="text-sm text-gray-600 mb-1">Critical</p>
            <p className="text-2xl font-bold text-red-600">
              {stats.bySeverity?.find(s => s.severity === 'Critical')?.count || 0}
            </p>
          </div>
          
          <div className="bg-white border border-gray-200 rounded-lg p-4 shadow-sm">
            <p className="text-sm text-gray-600 mb-1">High</p>
            <p className="text-2xl font-bold text-orange-600">
              {stats.bySeverity?.find(s => s.severity === 'High')?.count || 0}
            </p>
          </div>
          
          <div className="bg-white border border-gray-200 rounded-lg p-4 shadow-sm">
            <p className="text-sm text-gray-600 mb-1">Medium/Low</p>
            <p className="text-2xl font-bold text-green-600">
              {(stats.bySeverity?.find(s => s.severity === 'Medium')?.count || 0) + 
               (stats.bySeverity?.find(s => s.severity === 'Low')?.count || 0)}
            </p>
          </div>
        </div>
      )}

      {/* Filters */}
      <AuditFilters onFilterChange={handleFilterChange} onReset={handleResetFilters} />

      {/* Error Message */}
      {error && (
        <div className="bg-red-50 border border-red-200 rounded-lg p-4 mb-6">
          <div className="flex items-center gap-2">
            <svg className="w-5 h-5 text-red-600" fill="currentColor" viewBox="0 0 20 20">
              <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clipRule="evenodd" />
            </svg>
            <p className="text-red-800 font-medium">{error}</p>
          </div>
        </div>
      )}

      {/* Results Count */}
      {!loading && (
        <div className="mb-4 text-sm text-gray-600">
          Showing {auditLogs.length} of {totalCount} audit logs
        </div>
      )}

      {/* Timeline */}
      <AuditTimeline
        auditLogs={auditLogs}
        onViewDetails={handleViewDetails}
        loading={loading}
      />

      {/* Pagination */}
      {totalPages > 1 && (
        <div className="mt-8 flex items-center justify-center gap-2">
          <button
            onClick={() => setCurrentPage(prev => Math.max(1, prev - 1))}
            disabled={currentPage === 1}
            className="px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            Previous
          </button>
          
          <div className="flex items-center gap-1">
            {[...Array(Math.min(5, totalPages))].map((_, i) => {
              let pageNum;
              if (totalPages <= 5) {
                pageNum = i + 1;
              } else if (currentPage <= 3) {
                pageNum = i + 1;
              } else if (currentPage >= totalPages - 2) {
                pageNum = totalPages - 4 + i;
              } else {
                pageNum = currentPage - 2 + i;
              }

              return (
                <button
                  key={i}
                  onClick={() => setCurrentPage(pageNum)}
                  className={`px-3 py-1 rounded ${
                    currentPage === pageNum
                      ? 'bg-blue-600 text-white'
                      : 'border border-gray-300 hover:bg-gray-50'
                  }`}
                >
                  {pageNum}
                </button>
              );
            })}
          </div>

          <button
            onClick={() => setCurrentPage(prev => Math.min(totalPages, prev + 1))}
            disabled={currentPage === totalPages}
            className="px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
          >
            Next
          </button>
        </div>
      )}

      {/* Comparison Modal */}
      <AuditComparisonModal
        auditLog={selectedLog}
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
      />
    </div>
    </Layout>
  );
};

export default AuditLogsV3;
