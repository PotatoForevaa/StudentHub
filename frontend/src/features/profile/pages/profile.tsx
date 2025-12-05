import { useContext } from "react";
import { Container } from "../../../components/shared/Container";
import { AuthContext } from "../../../context/AuthContext";
import styled from "styled-components";

const Picture = styled.img`
        width: 300px;
        height: 300px;
        margin: 25px 0 0 25px;
        border-radius: 15%
    `;

export const Profile = () => {
  const { user, picture } = useContext(AuthContext);
  return (
    <Container>
      { picture && <Picture src={ picture }/> }
      { user && <div>{user.username}<span/>{user.fullName}</div>}
    </Container>
  );
};
