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
    <div className="space-y-8">
      {timelineData.map((group) => (
        <div key={group.date}>
          {/* Date header */}
          <div className="sticky top-0 z-10 bg-white border-b border-gray-200 py-3 mb-6">
            <h3 className="text-lg font-semibold text-gray-900">{formatDate(group.date)}</h3>
            <p className="text-sm text-gray-500">{group.logs.length} activities</p>
          </div>

          {/* Timeline items */}
          <div className="relative">
            {group.logs.map((log, index) => (
              <AuditCheckpoint
                key={log.id}
                log={log}
                onClick={() => onCheckpointClick(log.id)}
                isActive={selectedLogId === log.id}
              />
            ))}
          </div>
        </div>
      ))}
    </div>
  );
};

export default AuditTimeline;
