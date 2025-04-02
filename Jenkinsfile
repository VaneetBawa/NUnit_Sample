pipeline {
    agent any
    environment {
        BUILD_NAME = "jenkins-${env.JOB_NAME}-${env.BUILD_NUMBER}"
        LT_USERNAME = "vaneetb"
        LT_ACCESS_KEY = "PQ2AhuxqqOWAfvzfJlvPaOqusS7YKqfwFJR6DoWY2vsA8CvTzC"
        LT_BUILD_NAME = "${BUILD_NAME}"
    }
    stages {
        stage('Cleanup Workspace') {
            steps {
                deleteDir()
            }
        }
        stage('Restore Dependencies') {
            steps {
                sh '/usr/local/bin/dotnet restore'
            }
        }
        stage('Build') {
            steps {
                sh '/usr/local/bin/dotnet build --configuration Release'
            }
        }
        stage('Install Dependencies') {
            steps {
                sh '/usr/local/bin/dotnet tool install --global playwright-cli'
                sh 'playwright install'
            }
        }
        stage('Test') {
            steps {
                withEnv(["LT_USERNAME=${env.LT_USERNAME}", "LT_ACCESS_KEY=${env.LT_ACCESS_KEY}", "LT_BUILD_NAME=${env.BUILD_NAME}"]) {
                    sh '/usr/local/bin/dotnet test'
                }
            }
        }
    }
    post {
        always {
            lambdaTestReportPublisher 'automation'
            publishHTML(target: [
                allowMissing: false,
                alwaysLinkToLastBuild: true,
                keepAll: true,
                reportDir: 'TestResults/',
                reportFiles: 'TestLog.html, ExtentReports/API/Report.html, ExtentReports/UI/Report.html, Lighthouse-Reports/LH_Report.html',
                reportName: 'Execution Report',
                reportTitles: 'OverAll Report, API Report, UI Report, LightHouse Report'
            ])
        }
    }
}
