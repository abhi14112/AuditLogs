import React from 'react';
import AuditTimelineItem from './AuditTimelineItem';

/**
 * AuditTimeline - Timeline view for audit logs (Jira-style)
 */
const AuditTimeline = ({ auditLogs, onViewDetails, loading }) => {
  if (loading) {
    return (
      <div className="space-y-4">
        {[1, 2, 3, 4, 5].map((i) => (
          <div key={i} className="animate-pulse">
            <div className="flex gap-4 p-4 bg-white border border-gray-200 rounded-lg">
              <div className="w-10 h-10 bg-gray-300 rounded-full"></div>
              <div className="flex-1 space-y-2">
                <div className="h-4 bg-gray-300 rounded w-3/4"></div>
                <div className="h-3 bg-gray-200 rounded w-1/2"></div>
                <div className="h-3 bg-gray-200 rounded w-2/3"></div>
              </div>
            </div>
          </div>
        ))}
      </div>
    );
  }

  if (!auditLogs || auditLogs.length === 0) {
    return (
      <div className="text-center py-12">
        <div className="inline-flex items-center justify-center w-16 h-16 bg-gray-100 rounded-full mb-4">
          <svg
            className="w-8 h-8 text-gray-400"
            fill="none"
            stroke="currentColor"
            viewBox="0 0 24 24"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"
            />
          </svg>
        </div>
        <h3 className="text-lg font-medium text-gray-900 mb-1">No audit logs found</h3>
        <p className="text-gray-500">There are no audit logs matching your criteria.</p>
      </div>
    );
  }

  // Group by date
  const groupedByDate = auditLogs.reduce((groups, log) => {
    const date = new Date(log.createdAt).toLocaleDateString();
    if (!groups[date]) {
      groups[date] = [];
    }
    groups[date].push(log);
    return groups;
  }, {});

  return (
    <div className="space-y-6">
      {Object.entries(groupedByDate).map(([date, logs]) => (
        <div key={date}>
          {/* Date Header */}
          <div className="sticky top-0 z-10 bg-gray-50 border-b border-gray-200 px-4 py-2 mb-4">
            <h3 className="text-sm font-semibold text-gray-700">{date}</h3>
          </div>

          {/* Timeline Items */}
          <div className="space-y-4">
            {logs.map((log) => (
              <AuditTimelineItem
                key={log.id}
                auditLog={log}
                onViewDetails={onViewDetails}
              />
            ))}
          </div>
        </div>
      ))}
    </div>
  );
};

export default AuditTimeline;
