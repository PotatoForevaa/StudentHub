export type LeaderboardUser = {
    id: string;
    fullName: string;
    profilePicturePath: string;
    score: number;
}

export type LeaderboardType = 'rating' | 'activity';

export type LeaderboardPeriod = 'weekly-current' | 'weekly-last' | 'monthly-current' | 'monthly-last' | 'yearly-current';

export const LEADERBOARD_PERIODS: LeaderboardPeriod[] = [
    'weekly-current',
    'weekly-last',
    'monthly-current',
    'monthly-last',
    'yearly-current'
];

export const PERIOD_LABELS: Record<LeaderboardPeriod, string> = {
    'weekly-current': 'Текущая неделя',
    'weekly-last': 'Прошлая неделя',
    'monthly-current': 'Текущий месяц',
    'monthly-last': 'Прошлый месяц',
    'yearly-current': 'Текущий год'
};

export const TYPE_LABELS: Record<LeaderboardType, string> = {
    'rating': 'Топ по оценкам',
    'activity': 'Топ по активности'
};
