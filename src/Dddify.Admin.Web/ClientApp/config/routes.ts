export default [
  {
    path: '/auth',
    layout: false,
    routes: [
      {
        name: 'login',
        path: '/auth/login',
        component: './auth/login',
      },
    ],
  },
  {
    path: '/',
    redirect: '/welcome',
  },
  {
    path: '/welcome',
    name: 'welcome',
    icon: 'smile',
    component: './Welcome',
  },
  {
    path: '/system',
    name: 'system',
    icon: 'SettingOutlined',
    access: 'system',
    routes: [
      {
        path: '/system/user',
        name: 'user',
        component: './system/user',
        access: 'system:user:index',
      },
      {
        path: '/system/role',
        name: 'role',
        component: './system/role',
        access: 'system:role:index',
      },
      {
        path: '/system/permission',
        name: 'permission',
        component: './system/permission',
        access: 'system:permission:index',
      },
      {
        path: '/system/department',
        name: 'department',
        component: './system/department',
        access: 'system:department:index',
      },
      {
        path: '/system/lookup',
        name: 'lookup',
        component: './system/lookup',
        access: 'system:lookup:index',
      },
      {
        path: '/system/announcement',
        name: 'announcement',
        component: './system/announcement',
        access: 'system:announcement:index',
      },
    ],
  },
  {
    path: '/*',
    layout: false,
    component: './exception/404',
  },
];
