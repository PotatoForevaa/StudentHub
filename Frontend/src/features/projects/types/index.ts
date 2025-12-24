export type Project = {
    id: string;
    name?: string;
    description?: string;
    externalUrl?: string;
    creationDate: string;
    authorName: string;
    authorUsername: string;
    imagePaths?: string[];
    comments?: Comment[];
    averageRating: number;
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
}

export type ScoreFormData = {
    score: number;
}

export type ProjectFormData = {
    name: string;
    description: string;
    externalUrl: string;
}

export type CommentFormData = {
    content: string;
}

export type UpdateProjectFormData = {
    name: string;
    description: string;
    externalUrl?: string;
    base64Images?: string[];
}

export type FieldErrors = {
    [key: string]: string;
}
