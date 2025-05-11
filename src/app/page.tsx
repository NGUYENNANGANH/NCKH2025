'use client';

import Image from 'next/image';
import Link from 'next/link';
import ProductCard from '@/components/ProductCard';

const featuredProducts = [
  {
    id: 1,
    title: 'Wireless Headphones',
    price: 99.99,
    oldPrice: 129.99,
    image: '/images/products/headphones.jpg',
    rating: 4.5,
    reviews: 128,
  },
  {
    id: 2,
    title: 'Smart Watch',
    price: 199.99,
    oldPrice: 249.99,
    image: '/images/products/smartwatch.jpg',
    rating: 4.8,
    reviews: 256,
  },
  {
    id: 3,
    title: 'Bluetooth Speaker',
    price: 79.99,
    oldPrice: 99.99,
    image: '/images/products/speaker.jpg',
    rating: 4.3,
    reviews: 89,
  },
  {
    id: 4,
    title: 'Laptop Backpack',
    price: 49.99,
    oldPrice: 69.99,
    image: '/images/products/backpack.jpg',
    rating: 4.6,
    reviews: 156,
  },
];

const categories = [
  {
    id: 1,
    name: 'Electronics',
    image: '/images/categories/electronics.jpg',
    count: 150,
  },
  {
    id: 2,
    name: 'Fashion',
    image: '/images/categories/fashion.jpg',
    count: 200,
  },
  {
    id: 3,
    name: 'Home & Living',
    image: '/images/categories/home.jpg',
    count: 120,
  },
  {
    id: 4,
    name: 'Sports',
    image: '/images/categories/sports.jpg',
    count: 80,
  },
];

export default function Home() {
  return (
    <>
      {/* Hero Section */}
      <section className="hero position-relative">
        <Image
          src="/images/hero-bg.jpg"
          alt="Hero Background"
          width={1920}
          height={600}
          className="w-100"
          style={{ objectFit: 'cover', height: '600px' }}
        />
        <div className="position-absolute top-50 start-50 translate-middle text-center text-white">
          <h1 className="display-4 fw-bold mb-4">Welcome to eStore</h1>
          <p className="lead mb-4">
            Discover amazing products at unbeatable prices
          </p>
          <Link href="/category" className="btn btn-primary btn-lg">
            Shop Now
          </Link>
        </div>
      </section>

      {/* Categories Section */}
      <section className="section">
        <div className="container">
          <h2 className="text-center mb-5">Shop by Category</h2>
          <div className="row g-4">
            {categories.map((category) => (
              <div key={category.id} className="col-md-3">
                <Link
                  href={`/category/${category.id}`}
                  className="text-decoration-none"
                >
                  <div className="card h-100">
                    <Image
                      src={category.image}
                      alt={category.name}
                      width={300}
                      height={200}
                      className="card-img-top"
                      style={{ objectFit: 'cover', height: '200px' }}
                    />
                    <div className="card-body text-center">
                      <h5 className="card-title text-dark">{category.name}</h5>
                      <p className="card-text text-muted">
                        {category.count} Products
                      </p>
                    </div>
                  </div>
                </Link>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Featured Products Section */}
      <section className="section light-background">
        <div className="container">
          <h2 className="text-center mb-5">Featured Products</h2>
          <div className="row g-4">
            {featuredProducts.map((product) => (
              <div key={product.id} className="col-md-3">
                <ProductCard {...product} />
              </div>
            ))}
          </div>
          <div className="text-center mt-5">
            <Link href="/category" className="btn btn-outline-primary">
              View All Products
            </Link>
          </div>
        </div>
      </section>

      {/* Features Section */}
      <section className="section">
        <div className="container">
          <div className="row g-4">
            <div className="col-md-3">
              <div className="text-center">
                <i className="bi bi-truck display-4 text-primary mb-3"></i>
                <h5>Free Shipping</h5>
                <p className="text-muted">On all orders over $50</p>
              </div>
            </div>
            <div className="col-md-3">
              <div className="text-center">
                <i className="bi bi-arrow-return-left display-4 text-primary mb-3"></i>
                <h5>Easy Returns</h5>
                <p className="text-muted">30 days return policy</p>
              </div>
            </div>
            <div className="col-md-3">
              <div className="text-center">
                <i className="bi bi-shield-check display-4 text-primary mb-3"></i>
                <h5>Secure Payment</h5>
                <p className="text-muted">100% secure payment</p>
              </div>
            </div>
            <div className="col-md-3">
              <div className="text-center">
                <i className="bi bi-headset display-4 text-primary mb-3"></i>
                <h5>24/7 Support</h5>
                <p className="text-muted">Dedicated support</p>
              </div>
            </div>
          </div>
        </div>
      </section>
    </>
  );
} 