type OptionType = 'create' | 'update' | 'delete' | 'export';

type ApiResult<T> = {
    success: boolean;
    data?: T;
    errorCode?: string;
    errorMessage?: string;
    error?: any;
}

type Token = {
    accessToken?: string,
    refreshToken?: string
};

type User = {
    id: string;
    avatar?: string;
    fullName: string;
    nickName?: string;
    email: string;
    phoneNumber: string;
    gender: int;
    birthdDate?: string;
    status: int;
    type: string;
    concurrencyStamp?: string;
};

type Role = {
    id?: string;
    code: string;
    name: string;
    description?: string;
    concurrencyStamp?: string;
};

type Dictionary = {
    id: string;
    code: string;
    name: string;
    description?: string;
    concurrencyStamp?: string;
};

type DictionaryItem = {
    id: string;
    dictionaryId?: string;
    code: string;
    name: string;
    order?: number;
    isEnabled: boolean;
    concurrencyStamp?: string;
};

type Organization = {
    id?: string;
    parentId?: string;
    name: string;
    leader?: OrganizationLeader;
    order: number;
    isEnabled: boolean;
    concurrencyStamp?: string;
};

type OrganizationType = {
    code: string;
    name: string;
}