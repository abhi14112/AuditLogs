import React from 'react';

const AuditCheckpoint = ({ log, onClick, isActive }) => {
  const formatTime = (timestamp) => {
    const date = new Date(timestamp);
    return date.toLocaleTimeString('en-US', { 
      hour: '2-digit', 
      minute: '2-digit'
    });
  };

  const getActionColor = (actionType) => {
    const colors = {
      'Created': 'text-green-600 bg-green-100',
      'Updated': 'text-blue-600 bg-blue-100',
      'Deleted': 'text-red-600 bg-red-100',
      'Login': 'text-purple-600 bg-purple-100',
      'Logout': 'text-gray-600 bg-gray-100',
    };
    return colors[actionType] || 'text-gray-600 bg-gray-100';
  };

  return (
    <div 
      className={`relative pl-8 pb-8 cursor-pointer transition-all ${
        isActive ? 'opacity-100' : 'opacity-70 hover:opacity-100'
      }`}
      onClick={onClick}
    >
      {/* Timeline dot and line */}
      <div className="absolute left-0 top-2 flex flex-col items-center">
        <div className={`w-4 h-4 rounded-full border-4 ${
          isActive ? 'border-blue-600 bg-blue-600' : 'border-gray-400 bg-white'
        }`}></div>
        <div className="w-0.5 h-full bg-gray-300"></div>
      </div>

      {/* Content */}
      <div className={`bg-white rounded-lg shadow-sm border ${
        isActive ? 'border-blue-600 shadow-md' : 'border-gray-200 hover:border-gray-300'
      } p-4`}>
        <div className="flex items-start justify-between">
          <div className="flex-1">
            <div className="flex items-center gap-2 mb-2">
              <span className={`px-2 py-1 rounded text-xs font-medium ${getActionColor(log.actionType)}`}>
                {log.actionType}
              </span>
              <span className="text-xs text-gray-500">{formatTime(log.timestamp)}</span>
            </div>
            <p className="text-sm font-medium text-gray-900">{log.message}</p>
            <p className="text-xs text-gray-500 mt-1">{log.userName} • {log.entityName}</p>
          </div>
        </div>
      </div>
    </div>
  );
};

export default AuditCheckpoint;
