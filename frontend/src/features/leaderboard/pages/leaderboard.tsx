import { useState, useEffect } from "react";
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
  return (
    <Container>
      <Title>Рейтинг пользователей</Title>
      <SectionsContainer>
        <LeaderboardSection type="rating" />
        <LeaderboardSection type="activity" />
      </SectionsContainer>
    </Container>
  );
};

export default Leaderboard;
