import { useState } from "react";
import { Container } from "../../../shared/components/Container";
import { LoadingSpinner } from "../../../shared/components/LoadingSpinner";
import { leaderboardService } from "../services/leaderboardService";
import { LeaderboardSection } from "../components/LeaderboardSection";
import { styled } from "styled-components";
import { colors, fonts, spacing } from "../../../shared/styles/tokens";
import type { LeaderboardUser, LeaderboardType, LeaderboardPeriod } from "../types";

const Title = styled.h1`
  color: ${colors.textPrimary};
  font-size: ${fonts.size['2xl']};
  font-weight: ${fonts.weight.bold};
  margin: 0 0 ${spacing.xl} 0;
  text-align: center;
`;

const SectionsContainer = styled.div`
  display: flex;
  flex-direction: column;
  gap: ${spacing.xl};
`;

const Leaderboard = () => {
  const [page, setPage] = useState(0);

  return (
    <Container>
      <Title>Рейтинг пользователей</Title>
      <SectionsContainer>
<LeaderboardSection type="rating" />
<LeaderboardSection type="activity" />
      </SectionsContainer>
      <div style={{ display: 'flex', justifyContent: 'center', gap: '8px', marginTop: '16px' }}>
        <button
          disabled={page === 0}
          onClick={() => setPage(prev => Math.max(0, prev - 1))}
          style={{
            border: '1px solid #d1d5db',
            borderRadius: '4px',
            padding: '4px 12px',
            background: '#fff',
            cursor: page === 0 ? 'not-allowed' : 'pointer',
            opacity: page === 0 ? 0.5 : 1
          }}
        >
          Назад
        </button>
        <button
          onClick={() => setPage(prev => prev + 1)}
          style={{
            border: '1px solid #d1d5db',
            borderRadius: '4px',
            padding: '4px 12px',
            background: '#fff',
            cursor: 'pointer'
          }}
        >
          Вперёд
        </button>
      </div>
    </Container>
  );
};

export default Leaderboard;
