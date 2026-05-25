export type Project = {
    id: string;
    name?: string;
    description?: string;
    externalUrl?: string;
    creationDate: string;
    authorName: string;
    authorUsername: string;
    authorProfilePicturePath: string;
    imagePaths?: string[];
    comments?: Comment[];
    averageRating: number;
    categories?: CategoryDto[];
    tags?: TagDto[];
    criterionScores?: CriterionScoreDto[];
}

export type CategoryDto = {
    id: string;
    name: string;
}

export type TagDto = {
    id: string;
    name: string;
}

export type CriterionDto = {
    id: string;
    categoryId: string;
    categoryName: string;
    name: string;
}

export type CriterionScoreDto = {
    criterionId: string;
    criterionName: string;
    score: number;
    comment?: string;
    teacherName: string;
    createdAt: string;
}

export type Comment = {
    id: string;
    authorId: string;
    authorUsername: string;
    authorFullName: string;
    authorProfilePicturePath?: string;
    content: string;
    createdAt: string;
    userScore?: number;
    projectId?: string;
    projectName?: string;
    moderationStatus?: string;
    moderatedBy?: string;
    reportCount?: number;
    appealStatus?: string;
    appealMessage?: string;
}

export type ScoreFormData = {
    score: number;
}

export type ProjectFormData = {
    name: string;
    description: string;
    externalUrl: string;
    categoryIds?: string[];
    tagIds?: string[];
}

export type CommentFormData = {
    content: string;
}

export type UpdateProjectFormData = {
    name: string;
    description: string;
    externalUrl?: string;
    base64Images?: string[];
    categoryIds?: string[];
    tagIds?: string[];
}

export type FieldErrors = {
    [key: string]: string;
}

export type SubmitCriterionScoreData = {
    criterionId: string;
    score: number;
    comment?: string;
}

export type SubmitCriterionScoresData = {
    scores: SubmitCriterionScoreData[];
}