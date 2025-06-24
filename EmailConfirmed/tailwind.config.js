module.exports = {
    content: [
    ],
    theme: {
        extend: {
            fontFamily: {
                'poppins':['Poppins', 'sans-serif'],
                'inter':['Inter', 'sans-serif'],
                'roboto':['Roboto', 'sans-serif'],
            },
            keyframes: {
                wiggle: {
                    '0%, 100%': { transform: 'rotate(-10deg)' },
                    '50%': { transform: 'rotate(10deg)' }
                },
            },
            animation: {
                wiggle: 'wiggle 0.3s ease-in-out infinite'
            }
        }
    },
    plugins: []
} 