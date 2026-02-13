import React from 'react';

/**
 * AuditComparisonModal - Detailed view of audit log changes with before/after comparison
 */
const AuditComparisonModal = ({ auditLog, isOpen, onClose }) => {
  if (!isOpen || !auditLog) return null;

  // Parse old and new values
  const parseValues = () => {
    try {
      const oldVals = auditLog.oldValues ? JSON.parse(auditLog.oldValues) : {};
      const newVals = auditLog.newValues ? JSON.parse(auditLog.newValues) : {};
      
      // Get all unique keys
      const allKeys = [...new Set([...Object.keys(oldVals), ...Object.keys(newVals)])];
      
      return allKeys.map(key => ({
        field: key,
        oldValue: oldVals[key],
        newValue: newVals[key],
        changed: oldVals[key] !== newVals[key]
      }));
    } catch (error) {
      return [];
    }
  };

  const changes = parseValues();

  // Format value for display
  const formatValue = (value) => {
    if (value === null || value === undefined) return '—';
    if (typeof value === 'boolean') return value ? 'Yes' : 'No';
    if (typeof value === 'object') return JSON.stringify(value, null, 2);
    return String(value);
  };

  return (
    <div className="fixed inset-0 z-50 overflow-y-auto" onClick={onClose}>
      <div className="flex items-center justify-center min-h-screen px-4 pt-4 pb-20 text-center sm:block sm:p-0">
        {/* Background overlay */}
        <div className="fixed inset-0 transition-opacity bg-gray-500 bg-opacity-75" />

        {/* Center modal */}
        <span className="hidden sm:inline-block sm:align-middle sm:h-screen">&#8203;</span>

        {/* Modal panel */}
        <div
          className="inline-block align-bottom bg-white rounded-lg text-left overflow-hidden shadow-xl transform transition-all sm:my-8 sm:align-middle sm:max-w-4xl sm:w-full"
          onClick={(e) => e.stopPropagation()}
        >
          {/* Header */}
          <div className="bg-gradient-to-r from-blue-600 to-purple-600 px-6 py-4">
            <div className="flex items-center justify-between">
              <h3 className="text-xl font-semibold text-white">Audit Log Details</h3>
              <button
                onClick={onClose}
                className="text-white hover:text-gray-200 transition-colors"
              >
                <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                </svg>
              </button>
            </div>
          </div>

          {/* Content */}
          <div className="px-6 py-4 max-h-[70vh] overflow-y-auto">
            {/* Basic Information */}
            <div className="mb-6">
              <h4 className="text-lg font-semibold text-gray-900 mb-3">Event Information</h4>
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <p className="text-sm text-gray-600">User</p>
                  <p className="font-medium text-gray-900">{auditLog.username}</p>
                  <p className="text-xs text-gray-500">{auditLog.userEmail}</p>
                </div>
                <div>
                  <p className="text-sm text-gray-600">Date & Time</p>
                  <p className="font-medium text-gray-900">
                    {new Date(auditLog.createdAt).toLocaleString()}
                  </p>
                </div>
                <div>
                  <p className="text-sm text-gray-600">Action</p>
                  <p className="font-medium text-gray-900">{auditLog.action}</p>
                </div>
                <div>
                  <p className="text-sm text-gray-600">Entity Type</p>
                  <p className="font-medium text-gray-900">{auditLog.entityType}</p>
                </div>
                <div>
                  <p className="text-sm text-gray-600">Entity Name</p>
                  <p className="font-medium text-gray-900">{auditLog.entityName}</p>
                </div>
                <div>
                  <p className="text-sm text-gray-600">Severity</p>
                  <span className={`inline-block px-2 py-1 text-xs font-medium rounded ${
                    auditLog.severity === 'Critical' ? 'bg-red-100 text-red-800' :
                    auditLog.severity === 'High' ? 'bg-orange-100 text-orange-800' :
                    auditLog.severity === 'Medium' ? 'bg-yellow-100 text-yellow-800' :
                    'bg-green-100 text-green-800'
                  }`}>
                    {auditLog.severity}
                  </span>
                </div>
                {/* <div>
                  <p className="text-sm text-gray-600">IP Address</p>
                  <p className="font-medium text-gray-900">{auditLog.ipAddress}</p>
                </div> */}
                <div>
                  <p className="text-sm text-gray-600">Source</p>
                  <p className="font-medium text-gray-900">{auditLog.source}</p>
                </div>
                {/* {auditLog.correlationId && (
                  <div className="col-span-2">
                    <p className="text-sm text-gray-600">Correlation ID</p>
                    <p className="font-mono text-xs text-gray-900">{auditLog.correlationId}</p>
                  </div>
                )} */}
              </div>
            </div>

            {/* Description */}
            {auditLog.description && (
              <div className="mb-6">
                <h4 className="text-lg font-semibold text-gray-900 mb-2">Description</h4>
                <p className="text-gray-700 bg-gray-50 p-3 rounded">{auditLog.description}</p>
              </div>
            )}

            {/* Changes Comparison */}
            {changes.length > 0 && (
              <div>
                <h4 className="text-lg font-semibold text-gray-900 mb-3">Changes Comparison</h4>
                <div className="border border-gray-200 rounded-lg overflow-hidden">
                  <table className="min-w-full divide-y divide-gray-200">
                    <thead className="bg-gray-50">
                      <tr>
                        <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider w-1/4">
                          Field
                        </th>
                        <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider w-3/8">
                          Old Value
                        </th>
                        <th className="px-4 py-3 text-center text-xs font-medium text-gray-500 uppercase tracking-wider w-16">
                          →
                        </th>
                        <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider w-3/8">
                          New Value
                        </th>
                      </tr>
                    </thead>
                    <tbody className="bg-white divide-y divide-gray-200">
                      {changes.map((change, index) => (
                        <tr key={index} className={change.changed ? 'bg-yellow-50' : ''}>
                          <td className="px-4 py-3 text-sm font-medium text-gray-900">
                            {change.field}
                          </td>
                          <td className="px-4 py-3 text-sm text-gray-700">
                            <span className={change.changed ? 'line-through text-red-600' : ''}>
                              {formatValue(change.oldValue)}
                            </span>
                          </td>
                          <td className="px-4 py-3 text-center text-gray-400">
                            {change.changed ? '→' : ''}
                          </td>
                          <td className="px-4 py-3 text-sm">
                            <span className={change.changed ? 'font-semibold text-green-600' : 'text-gray-700'}>
                              {formatValue(change.newValue)}
                            </span>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              </div>
            )}

            {/* User Agent */}
            {auditLog.userAgent && (
              <div className="mt-6">
                <h4 className="text-sm font-semibold text-gray-900 mb-2">User Agent</h4>
                <p className="text-xs text-gray-600 bg-gray-50 p-2 rounded font-mono break-all">
                  {auditLog.userAgent}
                </p>
              </div>
            )}
          </div>

          {/* Footer */}
          <div className="bg-gray-50 px-6 py-4 flex justify-end">
            <button
              onClick={onClose}
              className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors font-medium"
            >
              Close
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default AuditComparisonModal;
