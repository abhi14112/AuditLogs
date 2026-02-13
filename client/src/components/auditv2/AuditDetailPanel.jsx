import React from 'react';

const AuditDetailPanel = ({ detail, onClose }) => {
  if (!detail) {
    return null;
  }

  const formatTimestamp = (timestamp) => {
    const date = new Date(timestamp);
    return date.toLocaleString('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
      second: '2-digit',
      hour12: true
    });
  };

  const renderValueComparison = () => {
    const oldValues = detail.oldValues || {};
    const newValues = detail.newValues || {};
    const changedFields = detail.changedFields || [];

    // Get all unique fields
    const allFields = new Set([
      ...Object.keys(oldValues),
      ...Object.keys(newValues)
    ]);

    return Array.from(allFields).map((field) => {
      const oldValue = oldValues[field];
      const newValue = newValues[field];
      const isChanged = changedFields.includes(field);

      return (
        <div 
          key={field} 
          className={`grid grid-cols-2 gap-4 p-4 rounded-lg ${
            isChanged ? 'bg-yellow-50 border border-yellow-200' : 'bg-gray-50'
          }`}
        >
          <div>
            <p className="text-xs font-medium text-gray-500 mb-1">{field}</p>
            <div className={`text-sm ${isChanged ? 'line-through text-red-600' : 'text-gray-900'}`}>
              {oldValue !== undefined && oldValue !== null ? String(oldValue) : '-'}
            </div>
          </div>
          <div>
            <p className="text-xs font-medium text-gray-500 mb-1">{field}</p>
            <div className={`text-sm font-medium ${isChanged ? 'text-green-600' : 'text-gray-900'}`}>
              {newValue !== undefined && newValue !== null ? String(newValue) : '-'}
            </div>
          </div>
        </div>
      );
    });
  };

  return (
    <div className="fixed inset-y-0 right-0 w-full md:w-2/5 bg-white shadow-2xl overflow-y-auto z-50 border-l border-gray-200">
      {/* Header */}
      <div className="sticky top-0 bg-white border-b border-gray-200 px-6 py-4 z-10">
        <div className="flex items-center justify-between">
          <h2 className="text-xl font-bold text-gray-900">Audit Details</h2>
          <button
            onClick={onClose}
            className="text-gray-400 hover:text-gray-600 transition-colors"
          >
            <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>
      </div>

      {/* Content */}
      <div className="px-6 py-6 space-y-6">
        {/* General Info */}
        <div className="bg-gradient-to-r from-blue-50 to-indigo-50 rounded-lg p-4 border border-blue-100">
          <h3 className="text-lg font-semibold text-gray-900 mb-3">{detail.message}</h3>
          <div className="grid grid-cols-2 gap-4 text-sm">
            <div>
              <p className="text-gray-500">User</p>
              <p className="font-medium text-gray-900">{detail.userName}</p>
            </div>
            <div>
              <p className="text-gray-500">Action</p>
              <p className="font-medium text-gray-900">{detail.actionType}</p>
            </div>
            <div>
              <p className="text-gray-500">Entity</p>
              <p className="font-medium text-gray-900">{detail.entityName}</p>
            </div>
            <div>
              <p className="text-gray-500">Timestamp</p>
              <p className="font-medium text-gray-900">{formatTimestamp(detail.timestamp)}</p>
            </div>
            <div>
              <p className="text-gray-500">IP Address</p>
              <p className="font-medium text-gray-900">{detail.ipAddress}</p>
            </div>
          </div>
        </div>

        {/* Before & After Comparison */}
        <div>
          <div className="flex items-center justify-between mb-4">
            <h4 className="text-lg font-semibold text-gray-900">Changes</h4>
            {detail.changedFields && detail.changedFields.length > 0 && (
              <span className="text-sm text-gray-500">
                {detail.changedFields.length} field(s) modified
              </span>
            )}
          </div>

          {/* Comparison Header */}
          <div className="grid grid-cols-2 gap-4 mb-3">
            <div className="text-center">
              <p className="text-xs font-semibold uppercase text-red-600 bg-red-50 py-2 rounded-t-lg">
                Before
              </p>
            </div>
            <div className="text-center">
              <p className="text-xs font-semibold uppercase text-green-600 bg-green-50 py-2 rounded-t-lg">
                After
              </p>
            </div>
          </div>

          {/* Values */}
          <div className="space-y-3">
            {renderValueComparison()}
          </div>

          {/* No changes message */}
          {(!detail.changedFields || detail.changedFields.length === 0) && (
            <div className="text-center py-8 text-gray-500">
              <p>No field changes detected</p>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default AuditDetailPanel;
