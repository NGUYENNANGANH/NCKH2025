# eStore Next.js Frontend

This is the frontend for the eStore e-commerce application built with Next.js and TypeScript. It connects to an ASP.NET Core backend API.

## Features

- Modern UI with Tailwind CSS
- Responsive design for all devices
- Product listing and details
- Shopping cart functionality
- User authentication
- Checkout process

## Prerequisites

- Node.js 18.x or later
- npm or yarn
- ASP.NET Core backend API running on http://localhost:5000

## Getting Started

1. Clone the repository
2. Install dependencies:
   ```bash
   npm install
   # or
   yarn install
   ```
3. Run the development server:
   ```bash
   npm run dev
   # or
   yarn dev
   ```
4. Open [http://localhost:3000](http://localhost:3000) in your browser

## Project Structure

- `/src/app` - Next.js app router pages
- `/src/components` - Reusable React components
- `/src/lib` - Utility functions and API services
- `/public` - Static assets

## API Integration

The frontend connects to the ASP.NET Core backend API. The API endpoints are configured in the `next.config.js` file to proxy requests to the backend.

## Technologies Used

- Next.js 14
- React 18
- TypeScript
- Tailwind CSS
- Axios for API requests

## License

This project is licensed under the MIT License. 