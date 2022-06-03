import React from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { HomePage } from './components/HomePage';
import './custom.css'

export default function App() {



    return (
        <div>
            <Layout>
                <Route exact path='/' render={() => <HomePage />} />
            </Layout>
        </div>
    );

}
