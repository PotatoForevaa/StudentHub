import React from 'react';
import styled from 'styled-components';
import { colors, fonts, spacing, borderRadius } from '../../../shared/styles/tokens';
import type { ActivityDto } from '../../../shared/types';

const ActivityCard = styled.div`
  padding: ${spacing.md};
  border: 1px solid ${colors.gray300};
  border-radius: ${borderRadius.md};
  background: ${colors.white};
  margin-bottom: ${spacing.sm};
  transition: box-shadow 0.2s ease;

  &:hover {
    box-shadow: 0 2px 8px rgba(0,0,0,0.1);
  }
`;

const ActivityHeader = styled.div`
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: ${spacing.sm};
`;

const ActivityType = styled.span<{ type: 'post' | 'comment' }>`
  background: ${props => props.type === 'post' ? colors.primary : '#f59e0b'};
  color: ${colors.white};
  padding: ${spacing.xs} ${spacing.sm};
  border-radius: ${borderRadius.sm};
  font-size: ${fonts.size.xs};
  font-weight: ${fonts.weight.semibold};
  text-transform: uppercase;
`;

const ActivityDate = styled.span`
  color: ${colors.gray500};
  font-size: ${fonts.size.sm};
`;

const ActivityContent = styled.div`
  color: ${colors.textPrimary};
  line-height: 1.5;
`;

const ProjectLink = styled.span`
  color: ${colors.primary};
  font-weight: ${fonts.weight.medium};
  margin-top: ${spacing.xs};
  display: block;
`;

interface ActivityItemProps {
  activity: ActivityDto;
}

export const ActivityItem: React.FC<ActivityItemProps> = ({ activity }) => {
  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return date.toLocaleDateString('ru-RU', {
      day: 'numeric',
      month: 'short',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  const getActivityText = () => {
    switch (activity.type) {
      case 'post':
        return `Создан пост: ${activity.title || activity.content}`;
      case 'comment':
        return `Комментарий: ${activity.content}`;
      default:
        return activity.content;
    }
  };

  return (
    <ActivityCard>
      <ActivityHeader>
        <ActivityType type={activity.type}>
          {activity.type === 'post' ? 'Пост' : 'Комментарий'}
        </ActivityType>
        <ActivityDate>{formatDate(activity.createdAt)}</ActivityDate>
      </ActivityHeader>
      <ActivityContent>
        {getActivityText()}
        {activity.projectName && (
          <ProjectLink>
            В проекте: {activity.projectName}
          </ProjectLink>
        )}
      </ActivityContent>
    </ActivityCard>
  );
};
