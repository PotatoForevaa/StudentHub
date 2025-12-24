import { useState, useEffect } from "react";
import { leaderboardService } from "../services/leaderboardService";
import { styled } from "styled-components";
import { colors, fonts, spacing, borderRadius, shadows } from "../../../shared/styles/tokens";
import { LoadingSpinner } from "../../../shared/components/LoadingSpinner";
import type { LeaderboardUser, LeaderboardType, LeaderboardPeriod } from "../types";
import { LEADERBOARD_PERIODS, PERIOD_LABELS, TYPE_LABELS } from "../types";
import { API_BASE_URL } from "../../../shared/services/base";

const SectionContainer = styled.div`
  background: ${colors.white};
  border-radius: ${borderRadius.lg};
  box-shadow: ${shadows.md};
  padding: ${spacing.xl};
`;

const SectionHeader = styled.div`
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: ${spacing.lg};
  flex-wrap: wrap;
  gap: ${spacing.md};
`;

const SectionTitle = styled.h2`
  color: ${colors.textPrimary};
  font-size: ${fonts.size.xl};
  font-weight: ${fonts.weight.bold};
  margin: 0;
`;

const PeriodSelector = styled.select`
  padding: ${spacing.sm} ${spacing.md};
  border: 1px solid ${colors.accentBorderLight};
  border-radius: ${borderRadius.md};
  background: ${colors.white};
  color: ${colors.textPrimary};
  font-size: ${fonts.size.base};
  cursor: pointer;
  transition: border-color 0.2s;

  &:hover {
    border-color: ${colors.primary};
  }

  &:focus {
    outline: none;
    border-color: ${colors.primary};
    box-shadow: 0 0 0 2px rgba(37, 99, 235, 0.1);
  }
`;

const LeaderboardList = styled.div`
  display: flex;
  flex-direction: column;
  gap: ${spacing.sm};
`;

const LeaderboardItem = styled.div`
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: ${spacing.md};
  background: ${colors.bg};
  border-radius: ${borderRadius.md};
  border: 1px solid ${colors.accentBorderLight};
  transition: all 0.2s;

  &:hover {
    background: ${colors.surface};
    transform: translateY(-1px);
    box-shadow: ${shadows.sm};
  }
`;

const Rank = styled.div`
  display: flex;
  align-items: center;
  justify-content: center;
  width: 40px;
  height: 40px;
  border-radius: 50%;
  background: linear-gradient(90deg, ${colors.primary}, ${colors.primaryDark});
  color: ${colors.white};
  font-weight: ${fonts.weight.bold};
  font-size: ${fonts.size.lg};
  margin-right: ${spacing.md};
`;

const UserInfo = styled.div`
  flex: 1;
  display: flex;
  align-items: center;
  gap: ${spacing.md};
`;

const UserAvatar = styled.img`
  width: 32px;
  height: 32px;
  border-radius: 50%;
  object-fit: cover;
  border: 1px solid ${colors.accentBorderLight};
`;

const UserName = styled.div`
  font-weight: ${fonts.weight.semibold};
  font-size: ${fonts.size.base};
  color: ${colors.textPrimary};
`;

const Score = styled.div`
  font-weight: ${fonts.weight.bold};
  font-size: ${fonts.size.lg};
  color: ${colors.primary};
`;

const EmptyState = styled.div`
  text-align: center;
  padding: ${spacing.xl};
  color: ${colors.textSecondary};
  font-size: ${fonts.size.base};
`;

interface LeaderboardSectionProps {
  type: LeaderboardType;
}

export const LeaderboardSection: React.FC<LeaderboardSectionProps> = ({ type }) => {
  const [users, setUsers] = useState<LeaderboardUser[]>([]);
  const [loading, setLoading] = useState(false);
  const [selectedPeriod, setSelectedPeriod] = useState<LeaderboardPeriod>('weekly-current');

  const fetchLeaderboard = async (period: LeaderboardPeriod) => {
    setLoading(true);
    try {
      const response = await leaderboardService.getLeaderboard(type, period);
      if (response.isSuccess && response.data) {
        setUsers(response.data);
      } else {
        setUsers([]);
      }
    } catch (error) {
      console.error('Failed to fetch leaderboard:', error);
      setUsers([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchLeaderboard(selectedPeriod);
  }, [selectedPeriod]);

  const handlePeriodChange = (event: React.ChangeEvent<HTMLSelectElement>) => {
    setSelectedPeriod(event.target.value as LeaderboardPeriod);
  };

  return (
    <SectionContainer>
      <SectionHeader>
        <SectionTitle>{TYPE_LABELS[type]}</SectionTitle>
        <PeriodSelector value={selectedPeriod} onChange={handlePeriodChange}>
          {LEADERBOARD_PERIODS.map(period => (
            <option key={period} value={period}>
              {PERIOD_LABELS[period]}
            </option>
          ))}
        </PeriodSelector>
      </SectionHeader>

      {loading ? (
        <LoadingSpinner text="Загрузка рейтинга..." size="md" />
      ) : users.length > 0 ? (
        <LeaderboardList>
          {users.map((user, index) => (
            <LeaderboardItem key={user.id}>
              <Rank>{index + 1}</Rank>
              <UserInfo>
                <UserAvatar
                  src={`http://localhost:5192/${user.profilePicturePath}`}
                  alt={`${user.fullName} avatar`}
                  onError={(e) => {
                    const target = e.target as HTMLImageElement;
                    target.src = `${API_BASE_URL}/api/users/by-username/default/profile-picture`;
                  }}
                />
                <UserName>{user.fullName}</UserName>
              </UserInfo>
              <Score>{Math.round(user.score * 100) / 100}</Score>
            </LeaderboardItem>
          ))}
        </LeaderboardList>
      ) : (
        <EmptyState>
          Нет данных для выбранного периода
        </EmptyState>
      )}
    </SectionContainer>
  );
};
