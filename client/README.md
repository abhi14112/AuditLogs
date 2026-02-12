# Inventory Management Client

A modern, responsive React application for managing inventory with a beautiful UI built using Vite, React, and Tailwind CSS.

## Features

- 🔐 **Authentication** - Secure login and registration
- 📦 **Product Management** - Create, read, update, and delete products
- 📊 **Audit Logs** - Track all changes and activities
- 🎨 **Modern UI** - Clean, responsive design with Tailwind CSS
- ⚡ **Fast Development** - Powered by Vite
- 🌐 **API Integration** - Seamless backend communication

## Tech Stack

- **React** - UI library
- **Vite** - Build tool and dev server
- **Tailwind CSS** - Utility-first CSS framework
- **React Router** - Client-side routing
- **Axios** - HTTP client
- **Lucide React** - Beautiful icons

## Getting Started

### Prerequisites

- Node.js 16+ and npm

### Installation

1. Navigate to the client directory:
```bash
cd client
```

2. Install dependencies (if not already installed):
```bash
npm install
```

3. Start the development server:
```bash
npm run dev
```

The application will open at `http://localhost:5173`

## Configuration

The API base URL is configured in `src/lib/api.js`. Update it to match your backend server:

```javascript
const API_BASE_URL = 'https://localhost:7001/api';
```

## Available Scripts

- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run preview` - Preview production build
- `npm run lint` - Run ESLint

## Project Structure

```
client/
├── public/              # Static assets
├── src/
│   ├── components/      # Reusable components
│   │   ├── Layout.jsx
│   │   └── ProtectedRoute.jsx
│   ├── context/         # React context providers
│   │   └── AuthContext.jsx
│   ├── lib/             # Utilities and configs
│   │   └── api.js
│   ├── pages/           # Page components
│   │   ├── Login.jsx
│   │   ├── Register.jsx
│   │   ├── Products.jsx
│   │   └── AuditLogs.jsx
│   ├── App.jsx          # Main app component
│   ├── main.jsx         # Entry point
│   └── index.css        # Global styles
├── tailwind.config.js   # Tailwind configuration
└── package.json
```

## Features Overview

### Authentication
- Login with username and password
- Register new user account
- Automatic token management
- Protected routes

### Products Management
- View all products in a responsive grid
- Search and filter products
- Add new products with name, description, quantity, and price
- Edit existing products
- Delete products with confirmation
- Real-time updates

### Audit Logs
- View all system activities
- Filter logs by user, action, or entity
- Color-coded action types (Create, Update, Delete)
- Formatted timestamps
- Detailed change tracking

## Customization

### Colors

The primary color scheme can be customized in `tailwind.config.js`:

```javascript
colors: {
  primary: {
    // Modify these values
    500: '#3b82f6',
    600: '#2563eb',
    700: '#1d4ed8',
  },
}
```

### Components

Custom component styles are defined in `src/index.css` using Tailwind's `@layer` directive:

- `.btn-primary` - Primary button style
- `.btn-secondary` - Secondary button style
- `.btn-danger` - Danger/delete button style
- `.input-field` - Form input style
- `.card` - Card container style

## Building for Production

1. Create a production build:
```bash
npm run build
```

2. The optimized files will be in the `dist` folder

3. Preview the production build locally:
```bash
npm run preview
```

## Deployment

The built files in `dist` can be deployed to any static hosting service:

- Vercel
- Netlify
- GitHub Pages
- Azure Static Web Apps
- AWS S3 + CloudFront

## License

MIT
