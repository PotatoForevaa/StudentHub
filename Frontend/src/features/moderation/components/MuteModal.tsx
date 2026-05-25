import { useState } from "react";
import styled from "styled-components";
import { moderationService } from "../services/moderationService";
import { colors, fonts, spacing, borderRadius, transitions } from "../../../shared/styles/tokens";

type MuteModalProps = {
  isOpen: boolean;
  onClose: () => void;
  userId: string;
  userName: string;
  onMuted: () => void;
};

const durationOptions = [
  { label: "1 час", value: "1h" },
  { label: "6 часов", value: "6h" },
  { label: "24 часа", value: "24h" },
  { label: "3 дня", value: "3d" },
  { label: "7 дней", value: "7d" },
  { label: "30 дней", value: "30d" },
];

const Overlay = styled.div`
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
`;

const Modal = styled.div`
  background: ${colors.white};
  border-radius: ${borderRadius.lg};
  padding: ${spacing.lg};
  min-width: 400px;
  max-width: 500px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.2);
`;

const Title = styled.h2`
  margin: 0 0 ${spacing.md};
  color: ${colors.textPrimary};
  font-size: ${fonts.size.xl};
`;

const Label = styled.label`
  display: block;
  margin-bottom: ${spacing.xs};
  font-weight: ${fonts.weight.semibold};
  color: ${colors.textPrimary};
  font-size: ${fonts.size.sm};
`;

const Select = styled.select`
  width: 100%;
  padding: ${spacing.sm};
  border: 1px solid ${colors.accentBorder};
  border-radius: ${borderRadius.sm};
  margin-bottom: ${spacing.md};
  font-family: inherit;
`;

const TextArea = styled.textarea`
  width: 100%;
  padding: ${spacing.sm};
  border: 1px solid ${colors.accentBorder};
  border-radius: ${borderRadius.sm};
  margin-bottom: ${spacing.md};
  font-family: inherit;
  resize: vertical;
  min-height: 80px;
`;

const ButtonRow = styled.div`
  display: flex;
  gap: ${spacing.sm};
  justify-content: flex-end;
`;

const Button = styled.button<{ danger?: boolean }>`
  border: none;
  border-radius: ${borderRadius.sm};
  padding: ${spacing.sm} ${spacing.md};
  color: ${colors.white};
  background: ${props => props.danger ? "#dc3545" : colors.primary};
  cursor: pointer;
  transition: filter ${transitions.base};
  &:hover { filter: brightness(0.95); }
  &:disabled { opacity: 0.6; cursor: not-allowed; }
`;

const CancelButton = styled.button`
  border: 1px solid ${colors.accentBorder};
  border-radius: ${borderRadius.sm};
  padding: ${spacing.sm} ${spacing.md};
  color: ${colors.textPrimary};
  background: ${colors.white};
  cursor: pointer;
  transition: filter ${transitions.base};
  &:hover { filter: brightness(0.95); }
`;

const ErrorMsg = styled.p`
  color: #dc3545;
  font-size: ${fonts.size.sm};
  margin: 0 0 ${spacing.sm};
`;

export const MuteModal = ({ isOpen, onClose, userId, userName, onMuted }: MuteModalProps) => {
  const [duration, setDuration] = useState("1h");
  const [reason, setReason] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  if (!isOpen) return null;

  const handleMute = async () => {
    setLoading(true);
    setError("");
    try {
      const result = await moderationService.muteUser(userId, duration, reason || undefined);
      if (result.isSuccess) {
        onMuted();
      } else {
        setError(result.errors?.map(e => e.message).join(", ") || "Не удалось заглушить пользователя");
      }
    } catch (err) {
      setError("Произошла ошибка при заглушении пользователя");
    } finally {
      setLoading(false);
    }
  };

  return (
    <Overlay onClick={onClose}>
      <Modal onClick={e => e.stopPropagation()}>
        <Title>Заглушить пользователя: {userName}</Title>
        
        {error && <ErrorMsg>{error}</ErrorMsg>}

        <Label htmlFor="duration">Длительность</Label>
        <Select id="duration" value={duration} onChange={e => setDuration(e.target.value)}>
          {durationOptions.map(opt => (
            <option key={opt.value} value={opt.value}>{opt.label}</option>
          ))}
        </Select>

        <Label htmlFor="reason">Причина (необязательно)</Label>
        <TextArea
          id="reason"
          value={reason}
          onChange={e => setReason(e.target.value)}
          placeholder="Почему этот пользователь заглушается?"
        />

        <ButtonRow>
          <CancelButton onClick={onClose} disabled={loading}>Отмена</CancelButton>
          <Button danger onClick={handleMute} disabled={loading}>
            {loading ? "Заглушение..." : "Заглушить пользователя"}
          </Button>
        </ButtonRow>
      </Modal>
    </Overlay>
  );
};