import { useState, useEffect } from 'react';
import { History, Search, Calendar } from 'lucide-react';
import api, { getErrorMessage } from '../lib/api';
import Layout from '../components/Layout';

const AuditLogs = () => {
  const [logs, setLogs] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [error, setError] = useState('');

  useEffect(() => {
    fetchLogs();
  }, []);

  const fetchLogs = async () => {
    try {
      setError('');
      const response = await api.get('/auditlogs');
      setLogs(Array.isArray(response.data) ? response.data : []);
    } catch (error) {
      const errorMsg = getErrorMessage(error);
      setError(errorMsg);
      console.error('Error fetching audit logs:', errorMsg);
      setLogs([]);
    } finally {
      setIsLoading(false);
    }
  };

  const filteredLogs = logs.filter((log) => {
    const action = log.action?.toLowerCase() || '';
    const username = log.username?.toLowerCase() || '';
    const entityName = log.entityName?.toLowerCase() || '';
    const search = searchTerm.toLowerCase();
    
    return action.includes(search) ||
           username.includes(search) ||
           entityName.includes(search);
  });

  const formatDate = (dateString) => {
    return new Date(dateString).toLocaleString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  const getActionColor = (action) => {
    switch (action.toLowerCase()) {
      case 'create':
      case 'created':
        return 'bg-green-100 text-green-800 border-green-200';
      case 'update':
      case 'updated':
        return 'bg-blue-100 text-blue-800 border-blue-200';
      case 'delete':
      case 'deleted':
        return 'bg-red-100 text-red-800 border-red-200';
      default:
        return 'bg-gray-100 text-gray-800 border-gray-200';
    }
  };

  return (
    <Layout>
      <div className="space-y-6">
        {/* Error Message */}
        {error && (
          <div className="p-4 bg-red-50 border border-red-200 text-red-700 rounded-lg flex justify-between items-center">
            <span>{error}</span>
            <button onClick={() => setError('')} className="text-red-700 hover:text-red-900 font-bold">
              ×
            </button>
          </div>
        )}

        {/* Header */}
        <div>
          <h2 className="text-3xl font-bold text-gray-900">Audit Logs</h2>
          <p className="text-gray-600 mt-1">Track all changes and activities</p>
        </div>

        {/* Search Bar */}
        <div className="card">
          <div className="relative">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400" size={20} />
            <input
              type="text"
              placeholder="Search logs..."
              className="input-field pl-10"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
            />
          </div>
        </div>

        {/* Logs List */}
        {isLoading ? (
          <div className="flex justify-center items-center py-12">
            <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-primary-600"></div>
          </div>
        ) : filteredLogs.length === 0 ? (
          <div className="card text-center py-12">
            <History className="mx-auto text-gray-400 mb-4" size={48} />
            <h3 className="text-lg font-medium text-gray-900 mb-2">No audit logs found</h3>
            <p className="text-gray-600">
              {searchTerm ? 'Try adjusting your search' : 'Activity logs will appear here'}
            </p>
          </div>
        ) : (
          <div className="card overflow-hidden p-0">
            <div className="overflow-x-auto">
              <table className="min-w-full divide-y divide-gray-200">
                <thead className="bg-gray-50">
                  <tr>
                    <th className="px-6 py-4 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      User
                    </th>
                    <th className="px-6 py-4 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Action
                    </th>
                    <th className="px-6 py-4 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Entity
                    </th>
                    <th className="px-6 py-4 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Changes
                    </th>
                    <th className="px-6 py-4 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Timestamp
                    </th>
                  </tr>
                </thead>
                <tbody className="bg-white divide-y divide-gray-200">
                  {filteredLogs.map((log) => (
                    <tr key={log.id} className="hover:bg-gray-50 transition-colors">
                      <td className="px-6 py-4 whitespace-nowrap">
                        <div className="flex items-center">
                          <div className="w-8 h-8 rounded-full bg-primary-100 text-primary-700 flex items-center justify-center font-medium">
                            {log.username ? log.username.charAt(0).toUpperCase() : '?'}
                          </div>
                          <div className="ml-3">
                            <p className="text-sm font-medium text-gray-900">{log.username || 'Unknown'}</p>
                          </div>
                        </div>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <span className={`px-3 py-1 inline-flex text-xs leading-5 font-semibold rounded-full border ${getActionColor(log.action || '')}`}>
                          {log.action || 'Unknown'}
                        </span>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                        {log.entityName || '-'}
                      </td>
                      <td className="px-6 py-4 text-sm text-gray-600 max-w-xs truncate">
                        {log.newValues || log.oldValues || '-'}
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                        <div className="flex items-center gap-2">
                          <Calendar size={14} />
                          {formatDate(log.timestamp)}
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        )}
      </div>
    </Layout>
  );
};

export default AuditLogs;
