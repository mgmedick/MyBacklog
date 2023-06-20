<template>
    <div>
        <div v-if="!isauth">          
            <span>Not logged in page</span>
        </div>  
        <div v-else>
            <span>Logged in page</span>
        </div>         
    </div>   
</template>
<script>
    export default {
        name: "Index",
        props: {
            isauth: Boolean
        },
        data: function () {
            return {
            };
        },
        watch: {},
        created: function () {
        },
        methods: {},
    };
</script>






