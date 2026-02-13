import React from 'react';

/**
 * AuditTimelineItem - Individual audit log entry in Jira-style timeline
 */
const AuditTimelineItem = ({ auditLog, onViewDetails }) => {
  // Parse changes if available
  const parseChanges = () => {
    if (!auditLog.oldValues || !auditLog.newValues) return [];
    
    try {
      const oldVals = JSON.parse(auditLog.oldValues);
      const newVals = JSON.parse(auditLog.newValues);
      
      const changes = [];
      Object.keys(newVals).forEach(key => {
        if (oldVals[key] !== newVals[key]) {
          changes.push({
            field: key,
            oldValue: oldVals[key],
            newValue: newVals[key]
          });
        }
      });
      
      return changes;
    } catch (error) {
      return [];
    }
  };

  const changes = parseChanges();
  
  // Format timestamp
  const formatTimestamp = (timestamp) => {
    const date = new Date(timestamp);
    const now = new Date();
    const diffMs = now - date;
    const diffMins = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMs / 3600000);
    const diffDays = Math.floor(diffMs / 86400000);
    
    if (diffMins < 1) return 'just now';
    if (diffMins < 60) return `${diffMins} minute${diffMins > 1 ? 's' : ''} ago`;
    if (diffHours < 24) return `${diffHours} hour${diffHours > 1 ? 's' : ''} ago`;
    if (diffDays < 7) return `${diffDays} day${diffDays > 1 ? 's' : ''} ago`;
    
    return date.toLocaleString();
  };

  // Get severity badge color
  const getSeverityColor = (severity) => {
    switch (severity?.toLowerCase()) {
      case 'critical':
        return 'bg-red-100 text-red-800 border-red-300';
      case 'high':
        return 'bg-orange-100 text-orange-800 border-orange-300';
      case 'medium':
        return 'bg-yellow-100 text-yellow-800 border-yellow-300';
      case 'low':
        return 'bg-green-100 text-green-800 border-green-300';
      default:
        return 'bg-gray-100 text-gray-800 border-gray-300';
    }
  };

  // Get action badge color
  const getActionColor = (action) => {
    switch (action?.toLowerCase()) {
      case 'create':
        return 'bg-blue-100 text-blue-800';
      case 'update':
        return 'bg-purple-100 text-purple-800';
      case 'delete':
        return 'bg-red-100 text-red-800';
      case 'login':
        return 'bg-green-100 text-green-800';
      case 'logout':
        return 'bg-gray-100 text-gray-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  };

  // Get user avatar initials
  const getInitials = (username) => {
    return username
      .split(' ')
      .map(n => n[0])
      .join('')
      .toUpperCase()
      .substring(0, 2);
  };

  return (
    <div className="flex gap-4 p-4 bg-white border border-gray-200 rounded-lg shadow-sm hover:shadow-md transition-shadow">
      {/* Avatar */}
      <div className="flex-shrink-0">
        <div className="w-10 h-10 bg-gradient-to-br from-blue-500 to-purple-600 rounded-full flex items-center justify-center text-white font-semibold">
          {getInitials(auditLog.username)}
        </div>
      </div>

      {/* Content */}
      <div className="flex-1 min-w-0">
        {/* Header */}
        <div className="flex items-start justify-between gap-2 mb-2">
          <div className="flex-1">
            <div className="flex items-center gap-2 flex-wrap">
              <span className="font-semibold text-gray-900">{auditLog.username}</span>
              <span className={`px-2 py-0.5 text-xs font-medium rounded ${getActionColor(auditLog.action)}`}>
                {auditLog.action}
              </span>
              <span className="text-gray-600">{auditLog.entityType}</span>
            </div>
            <p className="text-sm text-gray-600 mt-1">{auditLog.description}</p>
          </div>
          
          {/* Severity Badge */}
          <span className={`px-2 py-1 text-xs font-medium rounded border ${getSeverityColor(auditLog.severity)}`}>
            {auditLog.severity}
          </span>
        </div>

        {/* Changes Summary */}
        {auditLog.changesSummary && (
          <div className="bg-gray-50 border border-gray-200 rounded p-3 mb-2">
            <p className="text-sm font-medium text-gray-700 mb-1">Changes:</p>
            <p className="text-sm text-gray-600">{auditLog.changesSummary}</p>
          </div>
        )}

        {/* Detailed Changes */}
        {changes.length > 0 && (
          <div className="space-y-1 mb-2">
            {changes.slice(0, 3).map((change, index) => (
              <div key={index} className="flex items-center gap-2 text-sm">
                <span className="font-medium text-gray-700">{change.field}:</span>
                <span className="text-red-600 line-through">{String(change.oldValue)}</span>
                <span className="text-gray-400">→</span>
                <span className="text-green-600 font-medium">{String(change.newValue)}</span>
              </div>
            ))}
            {changes.length > 3 && (
              <button
                onClick={() => onViewDetails(auditLog)}
                className="text-sm text-blue-600 hover:text-blue-800 font-medium"
              >
                +{changes.length - 3} more changes
              </button>
            )}
          </div>
        )}

        {/* Footer */}
        <div className="flex items-center justify-between mt-3 pt-2 border-t border-gray-100">
          <div className="flex items-center gap-4 text-xs text-gray-500">
            <span title={new Date(auditLog.createdAt).toLocaleString()}>
              {formatTimestamp(auditLog.createdAt)}
            </span>
            {/* <span>IP: {auditLog.ipAddress}</span> */}
            {auditLog.source && <span>Source: {auditLog.source}</span>}
          </div>
          
          <button
            onClick={() => onViewDetails(auditLog)}
            className="text-xs text-blue-600 hover:text-blue-800 font-medium"
          >
            View Details
          </button>
        </div>
      </div>
    </div>
  );
};

export default AuditTimelineItem;
