import React from 'react';
import styled from 'styled-components';
import { ActivityItem } from './ActivityItem';
import { colors, spacing } from '../../../shared/styles/tokens';
import type { ActivityDto } from '../../../shared/types';

const ActivityContainer = styled.div`
  margin-top: ${spacing.xl};
`;

const ActivityTitle = styled.h3`
  color: ${colors.textPrimary};
  margin-bottom: ${spacing.md};
  font-size: 1.25rem;
  font-weight: 600;
`;

const LoadingText = styled.p`
  color: ${colors.muted};
  text-align: center;
  padding: ${spacing.xl};
`;

const EmptyText = styled.p`
  color: ${colors.muted};
  text-align: center;
  padding: ${spacing.xl};
`;

const ErrorText = styled.p`
  color: #dc2626;
  text-align: center;
  padding: ${spacing.xl};
`;

interface ActivityListProps {
  activities: ActivityDto[];
  loading: boolean;
  error: string | null;
  title?: string;
}

export const ActivityList: React.FC<ActivityListProps> = ({
  activities,
  loading,
  error,
  title = 'Недавняя активность'
}) => {
  if (loading) {
    return (
      <ActivityContainer>
        <ActivityTitle>{title}</ActivityTitle>
        <LoadingText>Загрузка активности...</LoadingText>
      </ActivityContainer>
    );
  }

  if (error) {
    return (
      <ActivityContainer>
        <ActivityTitle>{title}</ActivityTitle>
        <ErrorText>{error}</ErrorText>
      </ActivityContainer>
    );
  }

  if (!activities || activities.length === 0) {
    return (
      <ActivityContainer>
        <ActivityTitle>{title}</ActivityTitle>
        <EmptyText>Активности пока нет.</EmptyText>
      </ActivityContainer>
    );
  }

  return (
    <ActivityContainer>
      <ActivityTitle>{title}</ActivityTitle>
      {activities.map(activity => (
        <ActivityItem key={activity.id} activity={activity} />
      ))}
    </ActivityContainer>
  );
};
