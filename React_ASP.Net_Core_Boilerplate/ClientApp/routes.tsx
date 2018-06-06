import * as React from 'react';
import { Route, Redirect } from 'react-router-dom';
import { Layout } from './components/common/Layout';
import Home from './pages/HomePage/HomePage';

export const routes =
    <div>
        <Layout>        
            <Route exact path='/' component={Home} />
        </Layout>
</div>;
