import { useContext } from "react";
import { UserCard } from "../components/UserCard";
import { CardsContainer, Container } from "../../../shared/components/Container";
import { UserContext } from "../context/UserContext";
import { styled } from "styled-components";
import { colors, fonts } from "../../../shared/styles/tokens";

const Title = styled.h1`
  color: ${colors.textPrimary};
  font-size: ${fonts.size['2xl']};
  font-weight: ${fonts.weight.bold};
  margin: 0;
`;

const UserList = () => {
  const { users, loading } = useContext(UserContext);

  return (
    <Container>
      <Title>Пользователи</Title>

      <CardsContainer>
        {loading ? (
          <p>Загрузка пользователей...</p>
        ) : users && users.length > 0 ? (
          users.map((user) => <UserCard key={user.id} user={user} />)
        ) : (
          <p>Пользователи не найдены.</p>
        )}
      </CardsContainer>
    </Container>
  );
};

export default UserList;
