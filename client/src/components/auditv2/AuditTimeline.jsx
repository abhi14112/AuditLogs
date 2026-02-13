import React from 'react';
import AuditCheckpoint from './AuditCheckpoint';

const AuditTimeline = ({ timelineData, onCheckpointClick, selectedLogId }) => {
  const formatDate = (dateString) => {
    const date = new Date(dateString);
    const today = new Date();
    const yesterday = new Date(today);
    yesterday.setDate(yesterday.getDate() - 1);

    if (date.toDateString() === today.toDateString()) {
      return 'Today';
    } else if (date.toDateString() === yesterday.toDateString()) {
      return 'Yesterday';
    } else {
      return date.toLocaleDateString('en-US', { 
        month: 'long', 
        day: 'numeric', 
        year: 'numeric' 
      });
    }
  };

  if (!timelineData || timelineData.length === 0) {
    return (
      <div className="flex items-center justify-center h-64">
        <p className="text-gray-500">No audit logs found</p>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {timelineData.map((group, groupIndex) => (
        <div key={group.date}>
          {/* Date header as checkpoint */}
          <div className="mb-4">
            <div className="bg-gradient-to-r from-indigo-50 to-purple-50 rounded-lg p-3 border border-indigo-200 shadow-sm inline-flex items-center gap-2">
              <div className="w-5 h-5 rounded-full bg-indigo-600 flex items-center justify-center">
                <svg className="w-3 h-3 text-white" fill="currentColor" viewBox="0 0 20 20">
                  <path fillRule="evenodd" d="M6 2a1 1 0 00-1 1v1H4a2 2 0 00-2 2v10a2 2 0 002 2h12a2 2 0 002-2V6a2 2 0 00-2-2h-1V3a1 1 0 10-2 0v1H7V3a1 1 0 00-1-1zm0 5a1 1 0 000 2h8a1 1 0 100-2H6z" clipRule="evenodd" />
                </svg>
              </div>
              <div>
                <h3 className="text-base font-bold text-gray-900">{formatDate(group.date)}</h3>
                <p className="text-xs text-gray-600">{group.logs.length} activities</p>
              </div>
            </div>
          </div>

          {/* Timeline items */}
          <div className="space-y-3">
            {group.logs.map((log) => (
              <div 
                key={log.id}
                className={`bg-white rounded-lg shadow-sm border cursor-pointer transition-all ${
                  selectedLogId === log.id 
                    ? 'border-blue-600 shadow-md opacity-100' 
                    : 'border-gray-200 hover:border-gray-300 opacity-70 hover:opacity-100'
                } p-4`}
                onClick={() => onCheckpointClick(log.id)}
              >
                <div className="flex items-start justify-between">
                  <div className="flex-1">
                    <div className="flex items-center gap-2 mb-2">
                      <span className={`px-2 py-1 rounded text-xs font-medium ${
                        log.actionType === 'Created' ? 'text-green-600 bg-green-100' :
                        log.actionType === 'Updated' ? 'text-blue-600 bg-blue-100' :
                        log.actionType === 'Deleted' ? 'text-red-600 bg-red-100' :
                        'text-gray-600 bg-gray-100'
                      }`}>
                        {log.actionType}
                      </span>
                      <span className="text-xs text-gray-500">
                        {new Date(log.timestamp).toLocaleTimeString('en-US', { 
                          hour: '2-digit', 
                          minute: '2-digit',
                          hour12: true
                        })}
                      </span>
                    </div>
                    <p className="text-sm font-medium text-gray-900">{log.message}</p>
                    <p className="text-xs text-gray-500 mt-1">{log.userName} • {log.entityName}</p>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </div>
      ))}
    </div>
  );
};

export default AuditTimeline;
